using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.Services;
using Gemini.Modules.Shell.Views;
using Gemini.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gemini.Tests.Modules.Shell
{
    [STATestClass]
    [DoNotParallelize]
    public class LayoutItemStatePersisterTests
    {
        private static readonly byte[] ToolStateBytes = { 0x00, 0x2A, 0x80, 0xFF };
        private static readonly byte[] LayoutBytes = { 0xA5, 0x11, 0xEE, 0x5A };

        [TestMethod]
        public async Task LoadStateAsync_WhenStateFileIsAbsent_ReturnsNotFoundWithoutLoadingLayout()
        {
            using (var directory = new TestDirectory())
            {
                var stateFile = directory.GetPath("layout-state.bin");
                var shellView = new RecordingShellView(LayoutBytes);
                var persister = new LayoutItemStatePersister();

                var result = await persister.LoadStateAsync(
                    new TestShell(),
                    shellView,
                    stateFile);

                Assert.AreEqual(LayoutItemStateLoadStatus.NotFound, result.Status);
                Assert.IsNull(result.Exception);
                Assert.IsFalse(File.Exists(stateFile));
                Assert.AreEqual(0, shellView.LoadLayoutCallCount);
            }
        }

        [TestMethod]
        public async Task LoadStateAsync_UnreadableState_ReturnsFailedAndPreservesOriginal()
        {
            using (var directory = new TestDirectory())
            {
                var stateFile = directory.GetPath("layout-state.bin");
                var original = new byte[] { 0x01, 0x02, 0x03 };
                File.WriteAllBytes(stateFile, original);
                var persister = new LayoutItemStatePersister();
                LayoutItemStateLoadResult result;

                using (new FileStream(
                    stateFile,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.None))
                {
                    result = await persister.LoadStateAsync(
                        new TestShell(),
                        new RecordingShellView(LayoutBytes),
                        stateFile);
                }

                Assert.AreEqual(LayoutItemStateLoadStatus.Failed, result.Status);
                Assert.IsInstanceOfType<IOException>(result.Exception);
                CollectionAssert.AreEqual(original, File.ReadAllBytes(stateFile));
            }
        }

        [TestMethod]
        public async Task LoadStateAsync_TruncatedEnvelope_ReturnsCorruptAndPreservesOriginal()
        {
            using (var directory = new TestDirectory())
            {
                var stateFile = directory.GetPath("layout-state.bin");
                byte[] original;
                using (var stream = new MemoryStream())
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(1);
                    writer.Write("truncated type name");
                    writer.Flush();
                    original = stream.ToArray();
                }
                File.WriteAllBytes(stateFile, original);
                var persister = new LayoutItemStatePersister();

                var result = await persister.LoadStateAsync(
                    new TestShell(),
                    new RecordingShellView(LayoutBytes),
                    stateFile);

                Assert.AreEqual(LayoutItemStateLoadStatus.Corrupt, result.Status);
                Assert.IsNotNull(result.Exception);
                CollectionAssert.AreEqual(original, File.ReadAllBytes(stateFile));
            }
        }

        [TestMethod]
        public async Task LoadStateAsync_MissingItemType_ReturnsPartialAndKeepsLayoutReadable()
        {
            using (var directory = new TestDirectory())
            {
                var stateFile = directory.GetPath("layout-state.bin");
                var document = new PayloadDocument();
                WriteStateWithMissingAndUsableDocument(stateFile, LayoutBytes);
                var shellView = new RecordingShellView(LayoutBytes)
                {
                    LoadAction = (addTool, addDocument, items) =>
                        addDocument((IDocument)items["usable-document"])
                };
                var persister = new LayoutItemStatePersister();

                using (CreateIoCScope(type =>
                    type == typeof(PayloadDocument) ? document : null))
                {
                    var result = await persister.LoadStateAsync(
                        new TestShell(),
                        shellView,
                        stateFile);

                    Assert.AreEqual(LayoutItemStateLoadStatus.Partial, result.Status);
                    StringAssert.Contains(result.Details, "unavailable");
                    Assert.AreEqual(1, shellView.LoadLayoutCallCount);
                    CollectionAssert.AreEqual(LayoutBytes, shellView.LoadedLayoutBytes);
                    Assert.AreEqual(1, shellView.LastItems.Count);
                    Assert.AreEqual(42, document.Value);
                    Assert.AreEqual(1, result.RestorePlan.Documents.Count);
                    Assert.AreSame(document, result.RestorePlan.Documents[0]);
                    Assert.AreEqual(0, result.RestorePlan.VisibleTools.Count);
                }
            }
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task LoadStateAsync_AsyncItemRestore_RunsAfterLayoutAndCompletesBeforeSuccess()
        {
            using (var directory = new TestDirectory())
            {
                var stateFile = directory.GetPath("layout-state.bin");
                var payload = CreatePayload(writer => writer.Write("restored-file.cs"));
                WriteState(
                    stateFile,
                    typeof(DelayedRestoreDocument).AssemblyQualifiedName,
                    "delayed-document",
                    payload,
                    LayoutBytes);
                var document = new DelayedRestoreDocument();
                var shellView = new RecordingShellView(LayoutBytes)
                {
                    LoadAction = (addTool, addDocument, items) =>
                    {
                        var restored = (IDocument)items["delayed-document"];
                        restored.IsSelected = true;
                        addDocument(restored);
                    }
                };
                var persister = new LayoutItemStatePersister();

                using (CreateIoCScope(type =>
                    type == typeof(DelayedRestoreDocument) ? document : null))
                {
                    var operation = persister.LoadStateAsync(
                        new TestShell(),
                        shellView,
                        stateFile);
                    await document.RestoreEntered;

                    Assert.AreEqual(1, shellView.LoadLayoutCallCount);
                    Assert.IsTrue(shellView.StreamWasReadableDuringLoad);
                    Assert.IsFalse(operation.IsCompleted);
                    Assert.AreEqual("restored-file.cs", document.PendingPath);
                    Assert.IsNull(document.RestoredPath);

                    document.CompleteRestore();
                    var result = await operation;

                    Assert.AreEqual(LayoutItemStateLoadStatus.Success, result.Status);
                    Assert.AreEqual("restored-file.cs", document.RestoredPath);
                    Assert.AreEqual(1, result.RestorePlan.Documents.Count);
                    Assert.AreSame(document, result.RestorePlan.Documents[0]);
                    Assert.AreSame(document, result.RestorePlan.SelectedItem);
                }
            }
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task LoadStateAsync_AsyncItemRestoreFailure_ReturnsPartialAndExcludesFailedItem()
        {
            using (var directory = new TestDirectory())
            {
                var stateFile = directory.GetPath("layout-state.bin");
                var payload = CreatePayload(writer => writer.Write("missing-file.cs"));
                WriteState(
                    stateFile,
                    typeof(DelayedRestoreDocument).AssemblyQualifiedName,
                    "failed-document",
                    payload,
                    LayoutBytes);
                var document = new DelayedRestoreDocument();
                var shellView = new RecordingShellView(LayoutBytes)
                {
                    LoadAction = (addTool, addDocument, items) =>
                    {
                        var restored = (IDocument)items["failed-document"];
                        restored.IsSelected = true;
                        addDocument(restored);
                    }
                };
                var persister = new LayoutItemStatePersister();
                var expectedException = new IOException("content unavailable");

                using (CreateIoCScope(type =>
                    type == typeof(DelayedRestoreDocument) ? document : null))
                {
                    var operation = persister.LoadStateAsync(
                        new TestShell(),
                        shellView,
                        stateFile);
                    await document.RestoreEntered;

                    document.FailRestore(expectedException);
                    var result = await operation;

                    Assert.AreEqual(LayoutItemStateLoadStatus.Partial, result.Status);
                    Assert.AreSame(expectedException, result.Exception);
                    StringAssert.Contains(result.Details, "content unavailable");
                    Assert.AreEqual(0, result.RestorePlan.Documents.Count);
                    Assert.IsNull(result.RestorePlan.SelectedItem);
                }
            }
        }

        [TestMethod]
        public async Task LoadStateAsync_EqualDocuments_RegisterEachReferenceDuringLayoutCallback()
        {
            using (var directory = new TestDirectory())
            {
                var stateFile = directory.GetPath("layout-state.bin");
                using (var writer = new BinaryWriter(File.Create(stateFile)))
                {
                    writer.Write(2);
                    writer.Write(typeof(EqualDocument).AssemblyQualifiedName);
                    writer.Write("equal-1");
                    writer.Write(0L);
                    writer.Write(typeof(EqualDocument).AssemblyQualifiedName);
                    writer.Write("equal-2");
                    writer.Write(0L);
                    writer.Write(LayoutBytes);
                }

                var first = new EqualDocument();
                var second = new EqualDocument();
                var instances = new Queue<EqualDocument>(new[] { first, second });
                var shell = new TestShell();
                var registeredDuringCallback = new List<IDocument>();
                var shellView = new RecordingShellView(LayoutBytes)
                {
                    LoadAction = (addTool, addDocument, items) =>
                    {
                        addDocument((IDocument)items["equal-1"]);
                        addDocument((IDocument)items["equal-2"]);
                        registeredDuringCallback.AddRange(shell.Documents);
                    }
                };

                using (CreateIoCScope(type =>
                    type == typeof(EqualDocument) ? instances.Dequeue() : null))
                {
                    var result = await new LayoutItemStatePersister().LoadStateAsync(
                        shell,
                        shellView,
                        stateFile);

                    Assert.AreEqual(LayoutItemStateLoadStatus.Success, result.Status);
                    Assert.AreEqual(2, registeredDuringCallback.Count);
                    Assert.AreSame(first, registeredDuringCallback[0]);
                    Assert.AreSame(second, registeredDuringCallback[1]);
                    Assert.AreEqual(2, result.RestorePlan.Documents.Count);
                }
            }
        }

        // Intent: keep a duplicate content ID from replacing the first valid restored item.
        [TestMethod]
        public async Task LoadStateAsync_DuplicateContentId_ReturnsPartialAndKeepsFirstItem()
        {
            using (var directory = new TestDirectory())
            {
                string stateFile = directory.GetPath("layout-state.bin");
                using (var writer = new BinaryWriter(File.Create(stateFile)))
                {
                    writer.Write(2);
                    writer.Write(typeof(EqualDocument).AssemblyQualifiedName);
                    writer.Write("duplicate-document");
                    writer.Write(0L);
                    writer.Write(typeof(EqualDocument).AssemblyQualifiedName);
                    writer.Write("duplicate-document");
                    writer.Write(0L);
                    writer.Write(LayoutBytes);
                }

                var first = new EqualDocument();
                var second = new EqualDocument();
                var instances = new Queue<EqualDocument>(
                    new[] { first, second });
                var shell = new TestShell();
                var shellView = new RecordingShellView(LayoutBytes)
                {
                    LoadAction = (addTool, addDocument, items) =>
                        addDocument((IDocument)items["duplicate-document"])
                };

                using (CreateIoCScope(type =>
                    type == typeof(EqualDocument)
                        ? instances.Dequeue()
                        : null))
                {
                    var result = await new LayoutItemStatePersister().LoadStateAsync(
                        shell,
                        shellView,
                        stateFile);

                    Assert.AreEqual(LayoutItemStateLoadStatus.Partial, result.Status);
                    StringAssert.Contains(result.Details, "duplicate content ID");
                    Assert.AreEqual(1, shell.Documents.Count);
                    Assert.AreSame(first, shell.Documents[0]);
                    Assert.AreEqual(1, result.RestorePlan.Documents.Count);
                    Assert.AreSame(first, result.RestorePlan.Documents[0]);
                    Assert.AreEqual(0, instances.Count);
                }
            }
        }

        [TestMethod]
        public async Task LoadStateAsync_LayoutSerializerFailure_ReturnsCorruptWithOriginalException()
        {
            using (var directory = new TestDirectory())
            {
                var stateFile = directory.GetPath("layout-state.bin");
                WriteEmptyState(stateFile, LayoutBytes);
                var expectedException = new InvalidDataException("invalid layout xml");
                var shellView = new RecordingShellView(LayoutBytes)
                {
                    LoadException = expectedException
                };
                var persister = new LayoutItemStatePersister();

                var result = await persister.LoadStateAsync(
                    new TestShell(),
                    shellView,
                    stateFile);

                Assert.AreEqual(LayoutItemStateLoadStatus.Corrupt, result.Status);
                Assert.AreSame(expectedException, result.Exception);
            }
        }

        [TestMethod]
        public void SaveState_WithOneReopenableTool_WritesCompleteEnvelopeBeforeLayoutBytes()
        {
            using (var directory = new TestDirectory())
            {
                var stateFile = directory.GetPath("layout-state.bin");
                var tool = new TestTool(ToolStateBytes);
                var shellView = new RecordingShellView(LayoutBytes);
                var persister = new LayoutItemStatePersister();

                var result = persister.SaveState(
                    new TestShell(tools: new[] { tool }),
                    shellView,
                    stateFile);

                Assert.AreEqual(LayoutItemStateSaveStatus.Success, result.Status);
                Assert.AreEqual(1, tool.SaveStateCallCount);
                Assert.AreEqual(1, shellView.SaveLayoutCallCount);

                using (var reader = new BinaryReader(
                    new FileStream(stateFile, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    Assert.AreEqual(1, reader.ReadInt32());
                    Assert.AreEqual(typeof(TestTool).AssemblyQualifiedName, reader.ReadString());
                    Assert.AreEqual(tool.ContentId, reader.ReadString());
                    var payloadLength = reader.ReadInt64();
                    Assert.AreEqual(ToolStateBytes.LongLength, payloadLength);
                    CollectionAssert.AreEqual(
                        ToolStateBytes,
                        reader.ReadBytes(checked((int)payloadLength)));
                    CollectionAssert.AreEqual(LayoutBytes, reader.ReadBytes(LayoutBytes.Length));
                    Assert.AreEqual(reader.BaseStream.Length, reader.BaseStream.Position);
                }
            }
        }

        [TestMethod]
        public void SaveState_ItemPayloadFailure_IsPartialAndLeavesSkippableEnvelope()
        {
            using (var directory = new TestDirectory())
            {
                var stateFile = directory.GetPath("layout-state.bin");
                var expectedException = new InvalidOperationException("item save failed");
                var tool = new TestTool(ToolStateBytes)
                {
                    SaveException = expectedException
                };
                var persister = new LayoutItemStatePersister();

                var result = persister.SaveState(
                    new TestShell(tools: new[] { tool }),
                    new RecordingShellView(LayoutBytes),
                    stateFile);

                Assert.AreEqual(LayoutItemStateSaveStatus.Partial, result.Status);
                Assert.AreSame(expectedException, result.Exception);
                using (var reader = new BinaryReader(File.OpenRead(stateFile)))
                {
                    Assert.AreEqual(1, reader.ReadInt32());
                    reader.ReadString();
                    reader.ReadString();
                    Assert.AreEqual(0L, reader.ReadInt64());
                    CollectionAssert.AreEqual(LayoutBytes, reader.ReadBytes(LayoutBytes.Length));
                }
            }
        }

        [TestMethod]
        public void SaveState_ExistingDestination_ReplacesStateAndPreservesDeterministicBackup()
        {
            using (var directory = new TestDirectory())
            {
                var stateFile = directory.GetPath("layout-state.bin");
                var backupFile = stateFile + LayoutItemStatePersister.BackupFileSuffix;
                var oldState = new byte[] { 0x10, 0x20, 0x30 };
                File.WriteAllBytes(stateFile, oldState);
                File.WriteAllBytes(backupFile, new byte[] { 0xFF });
                var tool = new TestTool(ToolStateBytes);
                var persister = new LayoutItemStatePersister();

                var result = persister.SaveState(
                    new TestShell(tools: new[] { tool }),
                    new RecordingShellView(LayoutBytes),
                    stateFile);

                Assert.AreEqual(LayoutItemStateSaveStatus.Success, result.Status);
                CollectionAssert.AreEqual(oldState, File.ReadAllBytes(backupFile));
                CollectionAssert.AreNotEqual(oldState, File.ReadAllBytes(stateFile));
                Assert.IsFalse(File.Exists(
                    stateFile + LayoutItemStatePersister.TemporaryFileSuffix));
            }
        }

        [TestMethod]
        public void SaveState_ReplacementFailure_PreservesOldStateAndCleansOnlyKnownTemp()
        {
            using (var directory = new TestDirectory())
            {
                var stateFile = directory.GetPath("layout-state.bin");
                var oldState = new byte[] { 0x44, 0x55, 0x66 };
                File.WriteAllBytes(stateFile, oldState);
                var shellView = new RecordingShellView(LayoutBytes);
                var persister = new LayoutItemStatePersister();
                LayoutItemStateSaveResult result;

                using (new FileStream(
                    stateFile,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.None))
                {
                    result = persister.SaveState(
                        new TestShell(tools: new[] { new TestTool(ToolStateBytes) }),
                        shellView,
                        stateFile);
                }

                Assert.AreEqual(LayoutItemStateSaveStatus.Failed, result.Status);
                Assert.IsInstanceOfType<IOException>(result.Exception);
                Assert.AreEqual(1, shellView.SaveLayoutCallCount);
                CollectionAssert.AreEqual(oldState, File.ReadAllBytes(stateFile));
                Assert.IsFalse(File.Exists(
                    stateFile + LayoutItemStatePersister.TemporaryFileSuffix));
                Assert.IsFalse(File.Exists(
                    stateFile + LayoutItemStatePersister.BackupFileSuffix));
            }
        }

        private static byte[] CreatePayload(Action<BinaryWriter> write)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                write(writer);
                writer.Flush();
                return stream.ToArray();
            }
        }

        private static IoCOverrideScope CreateIoCScope(Func<Type, object> getInstance)
        {
            return new IoCOverrideScope(
                (type, key) => getInstance(type),
                type => Enumerable.Empty<object>(),
                instance => { });
        }

        private static void WriteEmptyState(string fileName, byte[] layoutBytes)
        {
            using (var writer = new BinaryWriter(File.Create(fileName)))
            {
                writer.Write(0);
                writer.Write(layoutBytes);
            }
        }

        private static void WriteStateWithMissingAndUsableDocument(
            string fileName,
            byte[] layoutBytes)
        {
            var usablePayload = CreatePayload(writer => writer.Write(42));
            using (var writer = new BinaryWriter(File.Create(fileName)))
            {
                writer.Write(2);
                writer.Write("Missing.Plugin.Tool, Missing.Plugin");
                writer.Write("missing-tool");
                writer.Write((long)ToolStateBytes.Length);
                writer.Write(ToolStateBytes);
                writer.Write(typeof(PayloadDocument).AssemblyQualifiedName);
                writer.Write("usable-document");
                writer.Write((long)usablePayload.Length);
                writer.Write(usablePayload);
                writer.Write(layoutBytes);
            }
        }

        private static void WriteState(
            string fileName,
            string typeName,
            string contentId,
            byte[] payload,
            byte[] layoutBytes)
        {
            using (var writer = new BinaryWriter(File.Create(fileName)))
            {
                writer.Write(1);
                writer.Write(typeName);
                writer.Write(contentId);
                writer.Write((long)payload.Length);
                writer.Write(payload);
                writer.Write(layoutBytes);
            }
        }

        private sealed class TestDirectory : IDisposable
        {
            public TestDirectory()
            {
                Path = System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(),
                    "gemini-state-tests-" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(Path);
            }

            public string Path { get; }

            public string GetPath(string fileName)
            {
                return System.IO.Path.Combine(Path, fileName);
            }

            public void Dispose()
            {
                if (Directory.Exists(Path))
                    Directory.Delete(Path, true);
            }
        }

        private sealed class TestTool : Screen, ITool
        {
            private readonly byte[] _stateBytes;

            public TestTool(byte[] stateBytes)
            {
                _stateBytes = stateBytes;
                Id = Guid.NewGuid();
                IsVisible = true;
            }

            public Guid Id { get; }

            public string ContentId => Id.ToString();

            public ICommand CloseCommand => null;

            public Uri IconSource => null;

            public bool IsSelected { get; set; }

            public bool ShouldReopenOnStart => true;

            public PaneLocation PreferredLocation => PaneLocation.Left;

            public double PreferredWidth => 320;

            public double PreferredHeight => 240;

            public bool IsVisible { get; set; }

            public Exception SaveException { get; set; }

            public int SaveStateCallCount { get; private set; }

            public void SaveState(BinaryWriter writer)
            {
                SaveStateCallCount++;
                if (SaveException != null)
                {
                    writer.Write((byte)0xFF);
                    throw SaveException;
                }
                writer.Write(_stateBytes);
            }

            public void LoadState(BinaryReader reader)
            {
            }
        }

        private sealed class PayloadDocument : Document
        {
            public int Value { get; private set; }

            public override void LoadState(BinaryReader reader)
            {
                Value = reader.ReadInt32();
            }
        }

        private sealed class EqualDocument : Document
        {
            public override bool Equals(object obj)
            {
                return obj is EqualDocument;
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        private sealed class DelayedRestoreDocument : Document, IAsyncLayoutItemStateRestorer
        {
            private readonly TaskCompletionSource<object> _restoreEntered =
                new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            private readonly TaskCompletionSource<object> _restoreGate =
                new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            public Task RestoreEntered => _restoreEntered.Task;

            public string PendingPath { get; private set; }

            public string RestoredPath { get; private set; }

            public override void LoadState(BinaryReader reader)
            {
                PendingPath = reader.ReadString();
            }

            public async Task RestoreStateAsync()
            {
                _restoreEntered.TrySetResult(null);
                await _restoreGate.Task;
                RestoredPath = PendingPath;
            }

            public void CompleteRestore()
            {
                _restoreGate.TrySetResult(null);
            }

            public void FailRestore(Exception exception)
            {
                _restoreGate.TrySetException(exception);
            }
        }

        private delegate void LayoutLoadAction(
            Action<ITool> addTool,
            Action<IDocument> addDocument,
            Dictionary<string, ILayoutItem> items);

        private sealed class RecordingShellView : IShellView
        {
            private readonly byte[] _layoutBytes;

            public RecordingShellView(byte[] layoutBytes)
            {
                _layoutBytes = layoutBytes;
            }

            public LayoutLoadAction LoadAction { get; set; }

            public Exception LoadException { get; set; }

            public Exception SaveException { get; set; }

            public int SaveLayoutCallCount { get; private set; }

            public int LoadLayoutCallCount { get; private set; }

            public byte[] LoadedLayoutBytes { get; private set; }

            public bool StreamWasReadableDuringLoad { get; private set; }

            public Dictionary<string, ILayoutItem> LastItems { get; private set; }

            public void LoadLayout(
                Stream stream,
                Action<ITool> addToolCallback,
                Action<IDocument> addDocumentCallback,
                Dictionary<string, ILayoutItem> itemsState)
            {
                LoadLayoutCallCount++;
                StreamWasReadableDuringLoad = stream.CanRead;
                LastItems = itemsState;
                using (var layout = new MemoryStream())
                {
                    stream.CopyTo(layout);
                    LoadedLayoutBytes = layout.ToArray();
                }

                if (LoadException != null)
                    throw LoadException;
                LoadAction?.Invoke(addToolCallback, addDocumentCallback, itemsState);
            }

            public void SaveLayout(Stream stream)
            {
                SaveLayoutCallCount++;
                if (SaveException != null)
                    throw SaveException;
                stream.Write(_layoutBytes, 0, _layoutBytes.Length);
            }

            public void UpdateFloatingWindows()
            {
            }
        }

        private sealed class TestShell : Screen, IShell
        {
            public TestShell(
                IEnumerable<IDocument> documents = null,
                IEnumerable<ITool> tools = null)
            {
                Documents = new BindableCollection<IDocument>(
                    documents ?? Enumerable.Empty<IDocument>());
                Tools = new BindableCollection<ITool>(tools ?? Enumerable.Empty<ITool>());
            }

            public event EventHandler ActiveDocumentChanging
            {
                add { }
                remove { }
            }

            public event EventHandler ActiveDocumentChanged
            {
                add { }
                remove { }
            }

            public bool ShowFloatingWindowsInTaskbar { get; set; }

            public Gemini.Modules.MainMenu.IMenu MainMenu => null;

            public Gemini.Modules.ToolBars.IToolBars ToolBars => null;

            public Gemini.Modules.StatusBar.IStatusBar StatusBar => null;

            public ILayoutItem ActiveLayoutItem { get; set; }

            public IDocument ActiveItem => ActiveLayoutItem as IDocument;

            public IObservableCollection<IDocument> Documents { get; }

            public IObservableCollection<ITool> Tools { get; }

            public Task InitializationTask => Task.CompletedTask;

            public bool RegisterTool(ITool tool)
            {
                if (Tools.Contains(tool))
                    return false;
                Tools.Add(tool);
                return true;
            }

            public Task ShowToolAsync<TTool>() where TTool : ITool
            {
                return Task.CompletedTask;
            }

            public Task ShowToolAsync(ITool model)
            {
                return Task.CompletedTask;
            }

            public Task CloseToolAsync(ITool tool)
            {
                return Task.CompletedTask;
            }

            public Task OpenDocumentAsync(IDocument model)
            {
                return Task.CompletedTask;
            }

            public Task CloseDocumentAsync(IDocument document)
            {
                return Task.CompletedTask;
            }

            public void Close()
            {
            }
        }
    }
}
