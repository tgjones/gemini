using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.Services;
using Gemini.Modules.Shell.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gemini.Tests.Modules.Shell
{
    [STATestClass]
    public class LayoutItemStatePersisterTests
    {
        private static readonly byte[] ToolStateBytes = { 0x00, 0x2A, 0x80, 0xFF };
        private static readonly byte[] LayoutBytes = { 0xA5, 0x11, 0xEE, 0x5A };

        [TestMethod]
        public void SaveState_WithOneReopenableTool_WritesItemEnvelopeBeforeLayoutBytes()
        {
            var testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var stateFile = Path.Combine(testDirectory, "layout-state.bin");
            var tool = new TestTool(ToolStateBytes);
            var shell = new TestShell(tool);
            var shellView = new RecordingShellView(LayoutBytes);
            var persister = new LayoutItemStatePersister();

            Directory.CreateDirectory(testDirectory);
            try
            {
                var result = persister.SaveState(shell, shellView, stateFile);

                Assert.IsTrue(result);
                Assert.AreEqual(1, tool.SaveStateCallCount);
                Assert.AreEqual(0, tool.LoadStateCallCount);
                Assert.AreEqual(1, shellView.SaveLayoutCallCount);
                Assert.AreEqual(0, shellView.LoadLayoutCallCount);
                Assert.AreEqual(0, shellView.LoadLayoutBytesConsumed);

                using (var reader = new BinaryReader(
                    new FileStream(stateFile, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    Assert.AreEqual(1, reader.ReadInt32());
                    Assert.AreEqual(typeof(TestTool).AssemblyQualifiedName, reader.ReadString());
                    Assert.AreEqual(TestTool.StableContentId, reader.ReadString());

                    var payloadLength = reader.ReadInt64();
                    Assert.AreEqual(ToolStateBytes.LongLength, payloadLength);

                    var payload = reader.ReadBytes(checked((int)payloadLength));
                    Assert.AreEqual(ToolStateBytes.Length, payload.Length);
                    Assert.AreEqual(ToolStateBytes[0], payload[0]);
                    Assert.AreEqual(ToolStateBytes[ToolStateBytes.Length - 1], payload[payload.Length - 1]);
                    CollectionAssert.AreEqual(ToolStateBytes, payload);

                    var layout = reader.ReadBytes(LayoutBytes.Length);
                    Assert.AreEqual(LayoutBytes.Length, layout.Length);
                    Assert.AreEqual(LayoutBytes[0], layout[0]);
                    Assert.AreEqual(LayoutBytes[LayoutBytes.Length - 1], layout[layout.Length - 1]);
                    CollectionAssert.AreEqual(LayoutBytes, layout);
                    Assert.AreEqual(reader.BaseStream.Length, reader.BaseStream.Position);
                    Assert.AreEqual(0, reader.ReadBytes(1).Length);
                }
            }
            finally
            {
                if (File.Exists(stateFile))
                    File.Delete(stateFile);
                if (Directory.Exists(testDirectory))
                    Directory.Delete(testDirectory);
            }
        }

        [TestMethod]
        public void LoadState_WhenStateFileIsAbsent_ReturnsFalseWithoutLoadingLayout()
        {
            var testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var stateFile = Path.Combine(testDirectory, "layout-state.bin");
            var tool = new TestTool(ToolStateBytes);
            var shell = new TestShell(tool);
            var shellView = new RecordingShellView(LayoutBytes);
            var persister = new LayoutItemStatePersister();

            Directory.CreateDirectory(testDirectory);
            try
            {
                Assert.IsFalse(File.Exists(stateFile));

                var result = persister.LoadState(shell, shellView, stateFile);

                Assert.IsFalse(result);
                Assert.IsFalse(File.Exists(stateFile));
                Assert.AreEqual(0, tool.SaveStateCallCount);
                Assert.AreEqual(0, tool.LoadStateCallCount);
                Assert.AreEqual(0, shellView.SaveLayoutCallCount);
                Assert.AreEqual(0, shellView.LoadLayoutCallCount);
                Assert.AreEqual(0, shellView.LoadLayoutBytesConsumed);
            }
            finally
            {
                if (File.Exists(stateFile))
                    File.Delete(stateFile);
                if (Directory.Exists(testDirectory))
                    Directory.Delete(testDirectory);
            }
        }

        private sealed class TestTool : Screen, ITool
        {
            public const string StableContentId = "test-tool-content";

            private readonly byte[] _stateBytes;

            public TestTool(byte[] stateBytes)
            {
                _stateBytes = stateBytes;
                Id = Guid.NewGuid();
                IsVisible = true;
            }

            public Guid Id { get; private set; }

            public string ContentId
            {
                get { return StableContentId; }
            }

            public ICommand CloseCommand
            {
                get { return null; }
            }

            public Uri IconSource
            {
                get { return null; }
            }

            public bool IsSelected { get; set; }

            public bool ShouldReopenOnStart
            {
                get { return true; }
            }

            public PaneLocation PreferredLocation
            {
                get { return PaneLocation.Left; }
            }

            public double PreferredWidth
            {
                get { return 320; }
            }

            public double PreferredHeight
            {
                get { return 240; }
            }

            public bool IsVisible { get; set; }

            public int SaveStateCallCount { get; private set; }

            public int LoadStateCallCount { get; private set; }

            public void SaveState(BinaryWriter writer)
            {
                SaveStateCallCount++;
                writer.Write(_stateBytes);
            }

            public void LoadState(BinaryReader reader)
            {
                LoadStateCallCount++;
            }
        }

        private sealed class RecordingShellView : IShellView
        {
            private readonly byte[] _layoutBytes;

            public RecordingShellView(byte[] layoutBytes)
            {
                _layoutBytes = layoutBytes;
            }

            public int SaveLayoutCallCount { get; private set; }

            public int LoadLayoutCallCount { get; private set; }

            public int LoadLayoutBytesConsumed { get; private set; }

            public void LoadLayout(
                Stream stream,
                Action<ITool> addToolCallback,
                Action<IDocument> addDocumentCallback,
                Dictionary<string, ILayoutItem> itemsState)
            {
                LoadLayoutCallCount++;
                var buffer = new byte[256];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    LoadLayoutBytesConsumed += bytesRead;
            }

            public void SaveLayout(Stream stream)
            {
                SaveLayoutCallCount++;
                stream.Write(_layoutBytes, 0, _layoutBytes.Length);
            }

            public void UpdateFloatingWindows()
            {
            }
        }

        private sealed class TestShell : Screen, IShell
        {
            public TestShell(ITool tool)
            {
                Documents = new BindableCollection<IDocument>();
                Tools = new BindableCollection<ITool> { tool };
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

            public Gemini.Modules.MainMenu.IMenu MainMenu
            {
                get { return null; }
            }

            public Gemini.Modules.ToolBars.IToolBars ToolBars
            {
                get { return null; }
            }

            public Gemini.Modules.StatusBar.IStatusBar StatusBar
            {
                get { return null; }
            }

            public ILayoutItem ActiveLayoutItem { get; set; }

            public IDocument ActiveItem
            {
                get { return ActiveLayoutItem as IDocument; }
            }

            public IObservableCollection<IDocument> Documents { get; private set; }

            public IObservableCollection<ITool> Tools { get; private set; }

            public System.Threading.Tasks.Task InitializationTask
            {
                get { return System.Threading.Tasks.Task.CompletedTask; }
            }

            public bool RegisterTool(ITool tool)
            {
                return false;
            }

            public System.Threading.Tasks.Task ShowToolAsync<TTool>()
                where TTool : ITool
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }

            public System.Threading.Tasks.Task ShowToolAsync(ITool model)
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }

            public System.Threading.Tasks.Task CloseToolAsync(ITool tool)
            {
                return System.Threading.Tasks.Task.CompletedTask;
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
