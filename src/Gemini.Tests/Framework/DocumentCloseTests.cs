using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Modules.UndoRedo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gemini.Tests.Framework
{
    [STATestClass]
    public class DocumentCloseTests
    {
        [TestMethod]
        [Timeout(10000)]
        public async Task TryCloseAsync_DirtySaveSuccess_ClosesAndDisposesUndoHistory()
        {
            var document = await CreateDirtyNewDocumentAsync(MessageBoxResult.Yes, "saved.txt");
            var undoAction = AddUndoAction(document);
            var conductor = await ActivateInConductorAsync(document);

            await document.TryCloseAsync();

            Assert.AreEqual(1, document.PromptCallCount);
            Assert.AreEqual(1, document.SaveAsPromptCallCount);
            Assert.AreEqual(1, document.SaveCallCount);
            Assert.IsFalse(document.IsDirty);
            Assert.IsFalse(document.IsNew);
            Assert.AreEqual("saved.txt", document.FilePath);
            Assert.IsTrue(undoAction.IsDisposed);
            Assert.IsFalse(document.IsActive);
            Assert.AreEqual(0, conductor.Items.Count);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task TryCloseAsync_DirtyDiscard_ClosesWithoutSavingAndDisposesUndoHistory()
        {
            var document = await CreateDirtyNewDocumentAsync(MessageBoxResult.No, null);
            var undoAction = AddUndoAction(document);
            var conductor = await ActivateInConductorAsync(document);

            await document.TryCloseAsync();

            Assert.AreEqual(1, document.PromptCallCount);
            Assert.AreEqual(0, document.SaveAsPromptCallCount);
            Assert.AreEqual(0, document.SaveCallCount);
            Assert.IsTrue(document.IsDirty);
            Assert.IsTrue(document.IsNew);
            Assert.IsTrue(undoAction.IsDisposed);
            Assert.AreEqual(0, conductor.Items.Count);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task TryCloseAsync_DirtyCancel_VetoesAndRetainsUndoHistory()
        {
            var document = await CreateDirtyNewDocumentAsync(MessageBoxResult.Cancel, null);
            var undoAction = AddUndoAction(document);
            var conductor = await ActivateInConductorAsync(document);

            await document.TryCloseAsync();

            Assert.AreEqual(1, document.PromptCallCount);
            Assert.AreEqual(0, document.SaveAsPromptCallCount);
            Assert.AreEqual(0, document.SaveCallCount);
            Assert.IsTrue(document.IsDirty);
            Assert.IsFalse(undoAction.IsDisposed);
            Assert.IsTrue(document.IsActive);
            Assert.AreSame(document, conductor.ActiveItem);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task TryCloseAsync_CancelledSaveAs_VetoesAndRemainsDistinctFromDirtyCancel()
        {
            var document = await CreateDirtyNewDocumentAsync(MessageBoxResult.Yes, null);
            var undoAction = AddUndoAction(document);
            var conductor = await ActivateInConductorAsync(document);

            await document.TryCloseAsync();

            Assert.AreEqual(1, document.PromptCallCount);
            Assert.AreEqual(1, document.SaveAsPromptCallCount);
            Assert.AreEqual(0, document.SaveCallCount);
            Assert.IsTrue(document.IsDirty);
            Assert.IsTrue(document.IsNew);
            Assert.IsFalse(undoAction.IsDisposed);
            Assert.IsTrue(document.IsActive);
            Assert.AreSame(document, conductor.ActiveItem);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task TryCloseAsync_SaveFailure_IsObservableAndRetainsUndoHistory()
        {
            var expectedException = new InvalidOperationException("save failed");
            var document = await CreateDirtyNewDocumentAsync(MessageBoxResult.Yes, "failed.txt");
            document.SaveOperation = () => Task.FromException(expectedException);
            var undoAction = AddUndoAction(document);
            var conductor = await ActivateInConductorAsync(document);

            var actualException = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () => await document.TryCloseAsync());

            Assert.AreSame(expectedException, actualException);
            Assert.AreEqual(1, document.PromptCallCount);
            Assert.AreEqual(1, document.SaveAsPromptCallCount);
            Assert.AreEqual(1, document.SaveCallCount);
            Assert.IsTrue(document.IsDirty);
            Assert.IsTrue(document.IsNew);
            Assert.AreEqual("failed.txt", document.FilePath);
            Assert.IsFalse(undoAction.IsDisposed);
            Assert.IsTrue(document.IsActive);
            Assert.AreSame(document, conductor.ActiveItem);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task TryCloseAsync_CancelledSaveOperation_PropagatesCancellationAndRetainsUndoHistory()
        {
            var document = await CreateDirtyNewDocumentAsync(MessageBoxResult.Yes, "cancelled.txt");
            document.SaveOperation = CreateCanceledTask;
            var undoAction = AddUndoAction(document);
            var conductor = await ActivateInConductorAsync(document);

            var closeTask = document.TryCloseAsync();
            await Assert.ThrowsExactlyAsync<TaskCanceledException>(async () => await closeTask);

            Assert.IsTrue(closeTask.IsCanceled);
            Assert.AreEqual(1, document.SaveCallCount);
            Assert.IsTrue(document.IsDirty);
            Assert.IsTrue(document.IsNew);
            Assert.IsFalse(undoAction.IsDisposed);
            Assert.IsTrue(document.IsActive);
            Assert.AreSame(document, conductor.ActiveItem);
        }

        private static async Task<TestPersistedDocument> CreateDirtyNewDocumentAsync(
            MessageBoxResult response,
            string saveAsPath)
        {
            var document = new TestPersistedDocument
            {
                CloseResponse = response,
                SaveAsPath = saveAsPath
            };
            await document.New("untitled.txt");
            document.IsDirty = true;
            return document;
        }

        private static DisposableUndoAction AddUndoAction(Document document)
        {
            var action = new DisposableUndoAction();
            document.UndoRedoManager.PushAction(action);
            return action;
        }

        private static async Task<Conductor<Document>.Collection.OneActive> ActivateInConductorAsync(
            Document document)
        {
            var conductor = new Conductor<Document>.Collection.OneActive();
            await ((IActivate)conductor).ActivateAsync(CancellationToken.None);
            await conductor.ActivateItemAsync(document, CancellationToken.None);
            return conductor;
        }

        private static Task CreateCanceledTask()
        {
            var completion = new TaskCompletionSource<object>();
            completion.SetCanceled();
            return completion.Task;
        }

        private sealed class TestPersistedDocument : PersistedDocument
        {
            public MessageBoxResult CloseResponse { get; set; }

            public string SaveAsPath { get; set; }

            public Func<Task> SaveOperation { get; set; } = () => Task.CompletedTask;

            public int PromptCallCount { get; private set; }

            public int SaveAsPromptCallCount { get; private set; }

            public int SaveCallCount { get; private set; }

            protected override MessageBoxResult PromptToSaveChanges()
            {
                PromptCallCount++;
                return CloseResponse;
            }

            protected override string PromptForSaveFilePath(IPersistedDocument persistedDocument)
            {
                SaveAsPromptCallCount++;
                return SaveAsPath;
            }

            protected override Task DoNew()
            {
                return Task.CompletedTask;
            }

            protected override Task DoLoad(string filePath)
            {
                return Task.CompletedTask;
            }

            protected override Task DoSave(string filePath)
            {
                SaveCallCount++;
                return SaveOperation();
            }
        }

        private sealed class DisposableUndoAction : IUndoableAction, IDisposable
        {
            public string Name => "test";

            public bool IsDisposed { get; private set; }

            public void Execute()
            {
            }

            public void Undo()
            {
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }
    }
}