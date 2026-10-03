using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Modules.MainMenu;
using Gemini.Modules.Shell.Commands;
using Gemini.Modules.Shell.Services;
using Gemini.Modules.StatusBar;
using Gemini.Modules.ToolBars;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gemini.Tests.Modules.Shell
{
    [STATestClass]
    public class EditorOpeningServiceTests
    {
        [TestMethod]
        [Timeout(10000)]
        public async Task OpenFileAsync_DelayedViewAttachmentAndLoad_AwaitsProviderOpen()
        {
            var shell = new TestShell();
            var document = new TestDocument();
            var providerGate = CreateCompletionSource();
            var provider = new TestEditorProvider(document)
            {
                OpenOperation = () => providerGate.Task
            };
            var service = new EditorOpeningService(shell, new[] { provider });

            var operation = service.OpenFileAsync("delayed.txt");

            Assert.IsNotNull(operation);
            Assert.AreEqual(1, shell.OpenDocumentCallCount);
            Assert.AreEqual(1, document.ViewAttachedSubscriberCount);
            Assert.AreEqual(0, provider.OpenCallCount);
            Assert.IsFalse(operation.IsCompleted);

            var view = new Border();
            document.AttachView(view);

            Assert.AreEqual(0, provider.OpenCallCount);
            Assert.IsFalse(operation.IsCompleted);

            RaiseLoaded(view);
            await provider.OpenEntered.Task;

            Assert.AreEqual(1, provider.OpenCallCount);
            Assert.AreSame(document, provider.OpenedDocument);
            Assert.AreEqual("delayed.txt", provider.OpenedPath);
            Assert.AreEqual(0, document.ViewAttachedSubscriberCount);
            Assert.IsFalse(operation.IsCompleted);

            providerGate.SetResult(null);
            var openedDocument = await operation;

            Assert.AreSame(document, openedDocument);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task OpenFileAsync_AlreadyLoadedView_InvokesProviderOpen()
        {
            var shell = new TestShell();
            var document = new TestDocument();
            var providerGate = CreateCompletionSource();
            var provider = new TestEditorProvider(document)
            {
                OpenOperation = () => providerGate.Task
            };
            var service = new EditorOpeningService(shell, new[] { provider });
            var view = new Border();
            var window = new Window
            {
                Content = view,
                Height = 1,
                ShowActivated = false,
                ShowInTaskbar = false,
                Width = 1,
                WindowStyle = WindowStyle.None
            };

            window.Show();
            try
            {
                Assert.IsTrue(view.IsLoaded);
                document.AttachView(view);

                var operation = service.OpenFileAsync("loaded.txt");

                Assert.AreEqual(1, provider.OpenCallCount);
                Assert.AreEqual(0, document.ViewAttachedSubscriberCount);
                Assert.IsFalse(operation.IsCompleted);

                providerGate.SetResult(null);
                window.Close();
                Assert.AreSame(document, await operation);
            }
            finally
            {
                if (window.IsVisible)
                    window.Close();
            }
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task OpenFileAsync_MissingProvider_ReturnsExplicitFailureTask()
        {
            var shell = new TestShell();
            var service = new EditorOpeningService(shell, new IEditorProvider[0]);

            var operation = service.OpenFileAsync("unsupported.none");

            Assert.IsNotNull(operation);
            var exception = await Assert.ThrowsExactlyAsync<NotSupportedException>(
                async () => await operation);
            StringAssert.Contains(exception.Message, "unsupported.none");
            Assert.AreEqual(0, shell.OpenDocumentCallCount);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task OpenFileAsync_UnsupportedPath_ReturnsExplicitFailureTask()
        {
            var shell = new TestShell();
            var provider = new TestEditorProvider(new TestDocument())
            {
                HandlesPath = path => false
            };
            var service = new EditorOpeningService(shell, new[] { provider });

            var operation = service.OpenFileAsync("unsupported.none");

            Assert.IsNotNull(operation);
            await Assert.ThrowsExactlyAsync<NotSupportedException>(
                async () => await operation);
            Assert.AreEqual(0, provider.CreateCallCount);
            Assert.AreEqual(0, shell.OpenDocumentCallCount);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task OpenFileAsync_ProviderFault_PropagatesAndDetachesCallbacks()
        {
            var expectedException = new InvalidOperationException("provider failed");
            var shell = new TestShell();
            var document = new TestDocument();
            var providerGate = CreateCompletionSource();
            var provider = new TestEditorProvider(document)
            {
                OpenOperation = () => providerGate.Task
            };
            var service = new EditorOpeningService(shell, new[] { provider });

            var operation = service.OpenFileAsync("fault.txt");
            var view = new Border();
            document.AttachView(view);
            RaiseLoaded(view);
            await provider.OpenEntered.Task;

            providerGate.SetException(expectedException);
            var actualException = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () => await operation);

            Assert.AreSame(expectedException, actualException);
            Assert.AreEqual(0, document.ViewAttachedSubscriberCount);
            Assert.IsTrue(operation.IsFaulted);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task OpenFileAsync_ProviderCancellation_PropagatesAndDetachesCallbacks()
        {
            var shell = new TestShell();
            var document = new TestDocument();
            var providerGate = CreateCompletionSource();
            var provider = new TestEditorProvider(document)
            {
                OpenOperation = () => providerGate.Task
            };
            var service = new EditorOpeningService(shell, new[] { provider });

            var operation = service.OpenFileAsync("cancelled.txt");
            var view = new Border();
            document.AttachView(view);
            RaiseLoaded(view);
            await provider.OpenEntered.Task;

            providerGate.SetCanceled();
            await Assert.ThrowsExactlyAsync<TaskCanceledException>(
                async () => await operation);

            Assert.AreEqual(0, document.ViewAttachedSubscriberCount);
            Assert.IsTrue(operation.IsCanceled);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task OpenFileAsync_ProviderFault_WaitsForPendingPresentation()
        {
            var providerException = new InvalidOperationException("provider failed");
            var presentationGate = CreateCompletionSource();
            var providerGate = CreateCompletionSource();
            var shell = new TestShell
            {
                OpenOperation = ignored => presentationGate.Task
            };
            var document = new TestDocument();
            var provider = new TestEditorProvider(document)
            {
                OpenOperation = () => providerGate.Task
            };
            var service = new EditorOpeningService(shell, new[] { provider });

            var operation = service.OpenFileAsync("provider-fault.txt");
            var view = new Border();
            document.AttachView(view);
            RaiseLoaded(view);
            await provider.OpenEntered.Task;

            providerGate.SetException(providerException);
            await Task.Yield();
            Assert.IsFalse(operation.IsCompleted);

            presentationGate.SetResult(null);
            var actualException = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () => await operation);
            Assert.AreSame(providerException, actualException);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task OpenFileAsync_PresentationFault_WaitsForPendingProvider()
        {
            var presentationException = new InvalidOperationException("presentation failed");
            var presentationGate = CreateCompletionSource();
            var providerGate = CreateCompletionSource();
            var shell = new TestShell
            {
                OpenOperation = ignored => presentationGate.Task
            };
            var document = new TestDocument();
            var provider = new TestEditorProvider(document)
            {
                OpenOperation = () => providerGate.Task
            };
            var service = new EditorOpeningService(shell, new[] { provider });

            var operation = service.OpenFileAsync("presentation-fault.txt");
            var view = new Border();
            document.AttachView(view);
            RaiseLoaded(view);
            await provider.OpenEntered.Task;

            presentationGate.SetException(presentationException);
            await Task.Yield();
            Assert.IsFalse(operation.IsCompleted);

            providerGate.SetResult(null);
            var actualException = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () => await operation);
            Assert.AreSame(presentationException, actualException);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task OpenFileAsync_ShellPresentationFailure_DetachesCallbacks()
        {
            var expectedException = new InvalidOperationException("presentation failed");
            var presentationGate = CreateCompletionSource();
            var shell = new TestShell
            {
                OpenOperation = ignored => presentationGate.Task
            };
            var document = new TestDocument();
            var provider = new TestEditorProvider(document);
            var service = new EditorOpeningService(shell, new[] { provider });

            var operation = service.OpenFileAsync("shell-fault.txt");

            Assert.AreEqual(1, document.ViewAttachedSubscriberCount);
            presentationGate.SetException(expectedException);
            var actualException = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () => await operation);

            Assert.AreSame(expectedException, actualException);
            Assert.AreEqual(0, document.ViewAttachedSubscriberCount);

            Assert.AreEqual(0, provider.OpenCallCount);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task NewFileCommandHandler_Run_AwaitsProviderNewAndIncrementsNameOnce()
        {
            var shell = new TestShell();
            var provider = new TestEditorProvider();
            var service = new EditorOpeningService(shell, new[] { provider });
            var handler = new NewFileCommandHandler(service, new[] { provider });
            var commands = new List<Command>();
            handler.Populate(
                new Command(new NewFileCommandListDefinition()),
                commands);
            var command = commands.Single();
            var providerGate = CreateCompletionSource();
            provider.NewOperation = () => providerGate.Task;
            var firstView = new Border();
            var duplicateView = new Border();
            var secondView = new Border();

            var firstRun = handler.Run(command);
            var firstDocument = provider.CreatedDocuments.Single();
            firstDocument.AttachView(firstView);
            RaiseLoaded(firstView);
            firstDocument.AttachView(duplicateView);
            RaiseLoaded(firstView);
            RaiseLoaded(duplicateView);

            var secondRun = handler.Run(command);
            var secondDocument = provider.CreatedDocuments[1];
            secondDocument.AttachView(secondView);
            RaiseLoaded(secondView);

            await provider.TwoNewCallsEntered.Task;

            Assert.AreEqual(2, provider.NewCallCount);
            CollectionAssert.AreEquivalent(
                new[] { "Untitled 1.txt", "Untitled 2.txt" },
                provider.NewNames);
            Assert.IsFalse(firstRun.IsCompleted);
            Assert.IsFalse(secondRun.IsCompleted);

            providerGate.SetResult(null);
            await Task.WhenAll(firstRun, secondRun);

            Assert.AreEqual(2, provider.NewCallCount);
            Assert.AreEqual(2, shell.OpenDocumentCallCount);
        }

        private static TaskCompletionSource<object> CreateCompletionSource()
        {
            return new TaskCompletionSource<object>(
                TaskCreationOptions.RunContinuationsAsynchronously);
        }

        private static void RaiseLoaded(FrameworkElement view)
        {
            view.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent, view));
        }

        private sealed class TestEditorProvider : IEditorProvider
        {
            private readonly Queue<TestDocument> _documents;

            public TestEditorProvider(params TestDocument[] documents)
            {
                _documents = new Queue<TestDocument>(documents);
                FileTypes = new[] { new EditorFileType("Text", ".txt") };
                HandlesPath = path => path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);
                OpenOperation = () => Task.CompletedTask;
                NewOperation = () => Task.CompletedTask;
                OpenEntered = CreateCompletionSource();
                TwoNewCallsEntered = CreateCompletionSource();
                CreatedDocuments = new List<TestDocument>();
                NewNames = new List<string>();
            }

            public IEnumerable<EditorFileType> FileTypes { get; }

            public bool CanCreateNew
            {
                get { return true; }
            }

            public Func<string, bool> HandlesPath { get; set; }

            public Func<Task> OpenOperation { get; set; }

            public Func<Task> NewOperation { get; set; }

            public TaskCompletionSource<object> OpenEntered { get; private set; }

            public TaskCompletionSource<object> TwoNewCallsEntered { get; }

            public List<TestDocument> CreatedDocuments { get; }

            public List<string> NewNames { get; }

            public int CreateCallCount { get; private set; }

            public int OpenCallCount { get; private set; }

            public int NewCallCount { get; private set; }

            public IDocument OpenedDocument { get; private set; }

            public string OpenedPath { get; private set; }

            public bool Handles(string path)
            {
                return HandlesPath(path);
            }

            public IDocument Create()
            {
                CreateCallCount++;
                var document = _documents.Count > 0
                    ? _documents.Dequeue()
                    : new TestDocument();
                CreatedDocuments.Add(document);
                return document;
            }

            public Task New(IDocument document, string name)
            {
                lock (NewNames)
                {
                    NewCallCount++;
                    NewNames.Add(name);
                    if (NewCallCount == 2)
                        TwoNewCallsEntered.TrySetResult(null);

                    return NewOperation();
                }
            }

            public Task Open(IDocument document, string path)
            {
                OpenCallCount++;
                OpenedDocument = document;
                OpenedPath = path;
                OpenEntered.TrySetResult(null);
                return OpenOperation();
            }
        }

        private sealed class TestDocument : Document, IViewAware
        {
            private EventHandler<ViewAttachedEventArgs> _viewAttached;
            private object _view;

            public int ViewAttachedSubscriberCount
            {
                get
                {
                    return _viewAttached == null
                        ? 0
                        : _viewAttached.GetInvocationList().Length;
                }
            }

            event EventHandler<ViewAttachedEventArgs> IViewAware.ViewAttached
            {
                add { _viewAttached += value; }
                remove { _viewAttached -= value; }
            }

            void IViewAware.AttachView(object view, object context)
            {
                _view = view;
                _viewAttached?.Invoke(
                    this,
                    new ViewAttachedEventArgs
                    {
                        Context = context,
                        View = view
                    });
            }

            object IViewAware.GetView(object context)
            {
                return _view;
            }

            public void AttachView(object view)
            {
                ((IViewAware)this).AttachView(view);
            }
        }

        private sealed class TestShell : Screen, IShell
        {
            public TestShell()
            {
                Documents = new BindableCollection<IDocument>();
                Tools = new BindableCollection<ITool>();
                OpenOperation = document => Task.CompletedTask;
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

            public IMenu MainMenu
            {
                get { return null; }
            }

            public IToolBars ToolBars
            {
                get { return null; }
            }

            public IStatusBar StatusBar
            {
                get { return null; }
            }

            public ILayoutItem ActiveLayoutItem { get; set; }

            public IDocument ActiveItem
            {
                get { return ActiveLayoutItem as IDocument; }
            }

            public IObservableCollection<IDocument> Documents { get; }

            public IObservableCollection<ITool> Tools { get; }

            public Task InitializationTask
            {
                get { return Task.CompletedTask; }
            }

            public Func<IDocument, Task> OpenOperation { get; set; }

            public int OpenDocumentCallCount { get; private set; }

            public bool RegisterTool(ITool tool)
            {
                return false;
            }

            public Task ShowToolAsync<TTool>()
                where TTool : ITool
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
                OpenDocumentCallCount++;
                return OpenOperation(model);
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
