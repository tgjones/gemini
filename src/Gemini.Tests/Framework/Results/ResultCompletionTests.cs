using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Gemini.Framework.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gemini.Tests.Framework.Results
{
    [TestClass]
    public class ResultCompletionTests
    {
        [TestMethod]
        public void LambdaResult_Execute_InvokesActionBeforeSingleSuccessfulCompletion()
        {
            var markers = new List<string>();
            var actionCount = 0;
            var completionCount = 0;
            CoroutineExecutionContext actionContext = null;
            object completionSender = null;
            ResultCompletionEventArgs completionArgs = null;
            var result = new LambdaResult(delegate(CoroutineExecutionContext context)
            {
                actionCount++;
                actionContext = context;
                markers.Add("Action");
            });
            result.Completed += delegate(object sender, ResultCompletionEventArgs eventArgs)
            {
                completionCount++;
                completionSender = sender;
                completionArgs = eventArgs;
                markers.Add("Completed");
            };

            var executionContext = new CoroutineExecutionContext();
            ((IResult)result).Execute(executionContext);

            CollectionAssert.AreEqual(
                new[] { "Action", "Completed" },
                markers);
            Assert.AreEqual(1, actionCount);
            Assert.AreSame(executionContext, actionContext);
            Assert.AreEqual(1, completionCount);
            Assert.AreSame(result, completionSender);
            Assert.IsNotNull(completionArgs);
            Assert.IsNull(completionArgs.Error);
            Assert.IsFalse(completionArgs.WasCancelled);
        }

        [STATestMethod]
        [Timeout(10000)]
        public async System.Threading.Tasks.Task OpenDocumentResult_Execute_WithProvidedDocument_ConfiguresBeforeOpenAndCompletesAfterOpenTask()
        {
            var markers = new List<string>();
            var document = new TestDocument();
            var shell = new GatedShell(markers);
            var result = new OpenDocumentResult(document);
            var completionEvent = new System.Threading.Tasks.TaskCompletionSource<object>(
                System.Threading.Tasks.TaskCreationOptions.RunContinuationsAsynchronously);
            var configureCount = 0;
            var completionCount = 0;
            Gemini.Framework.IDocument configuredDocument = null;
            object completionSender = null;
            ResultCompletionEventArgs completionArgs = null;
            ((IOpenResult<Gemini.Framework.IDocument>)result).OnConfigure =
                delegate(Gemini.Framework.IDocument configured)
                {
                    configureCount++;
                    configuredDocument = configured;
                    markers.Add("Configure");
                };
            result.Completed += delegate(object sender, ResultCompletionEventArgs eventArgs)
            {
                completionCount++;
                completionSender = sender;
                completionArgs = eventArgs;
                markers.Add("Completed");
                completionEvent.SetResult(null);
            };

            using (ComposeShell(result, shell))
            {
                ((IResult)result).Execute(new CoroutineExecutionContext());
                await shell.OpenEntered;

                CollectionAssert.AreEqual(
                    new[] { "Configure", "Open" },
                    markers);
                Assert.AreEqual(1, configureCount);
                Assert.AreEqual(1, shell.OpenDocumentCallCount);
                Assert.AreSame(document, configuredDocument);
                Assert.AreSame(document, shell.OpenedDocument);
                Assert.AreEqual(0, completionCount);
                Assert.IsFalse(completionEvent.Task.IsCompleted);

                shell.CompleteOpen();
                await completionEvent.Task;
            }

            CollectionAssert.AreEqual(
                new[] { "Configure", "Open", "Completed" },
                markers);
            Assert.AreEqual(1, configureCount);
            Assert.AreEqual(1, shell.OpenDocumentCallCount);
            Assert.AreSame(document, configuredDocument);
            Assert.AreSame(document, shell.OpenedDocument);
            Assert.AreEqual(1, completionCount);
            Assert.AreSame(result, completionSender);
            Assert.IsNotNull(completionArgs);
            Assert.IsNull(completionArgs.Error);
            Assert.IsFalse(completionArgs.WasCancelled);
        }

        [STATestMethod]
        [Timeout(10000)]
        public async System.Threading.Tasks.Task OpenDocumentResult_Execute_WithPath_CompletesAfterEditorOperation()
        {
            var markers = new List<string>();
            var document = new TestDocument();
            var shell = new GatedShell(markers);
            var editorOpeningService = new GatedEditorOpeningService(document, markers);
            var result = new OpenDocumentResult("document.txt");
            var completionEvent = new System.Threading.Tasks.TaskCompletionSource<object>(
                System.Threading.Tasks.TaskCreationOptions.RunContinuationsAsynchronously);
            var completionCount = 0;
            ResultCompletionEventArgs completionArgs = null;
            ((IOpenResult<Gemini.Framework.IDocument>)result).OnConfigure =
                delegate(Gemini.Framework.IDocument configured)
                {
                    Assert.AreSame(document, configured);
                    markers.Add("Configure");
                };
            result.Completed += delegate(object sender, ResultCompletionEventArgs eventArgs)
            {
                completionCount++;
                completionArgs = eventArgs;
                markers.Add("Completed");
                completionEvent.SetResult(null);
            };

            using (ComposeShell(result, shell, editorOpeningService))
            {
                ((IResult)result).Execute(new CoroutineExecutionContext());
                await editorOpeningService.OpenEntered;

                CollectionAssert.AreEqual(
                    new[] { "Configure", "EditorOperation" },
                    markers);
                Assert.AreEqual("document.txt", editorOpeningService.OpenedPath);
                Assert.AreEqual(0, shell.OpenDocumentCallCount);
                Assert.AreEqual(0, completionCount);

                editorOpeningService.CompleteOpen();
                await completionEvent.Task;
            }

            CollectionAssert.AreEqual(
                new[] { "Configure", "EditorOperation", "Completed" },
                markers);
            Assert.AreEqual(1, completionCount);
            Assert.IsNotNull(completionArgs);
            Assert.IsNull(completionArgs.Error);
            Assert.IsFalse(completionArgs.WasCancelled);
        }

        [STATestMethod]
        [DoNotParallelize]
        [Timeout(10000)]
        public async System.Threading.Tasks.Task OpenDocumentResult_Execute_WithNullProvidedDocument_CompletesOnceAsCancelled()
        {
            var markers = new List<string>();
            var shell = new GatedShell(markers);
            var result = new OpenDocumentResult((Gemini.Framework.IDocument)null);
            var completionEvent = new System.Threading.Tasks.TaskCompletionSource<object>(
                System.Threading.Tasks.TaskCreationOptions.RunContinuationsAsynchronously);
            var completionCount = 0;
            var getInstanceCount = 0;
            object completionSender = null;
            ResultCompletionEventArgs completionArgs = null;
            result.Completed += delegate(object sender, ResultCompletionEventArgs eventArgs)
            {
                completionCount++;
                completionSender = sender;
                completionArgs = eventArgs;
                completionEvent.SetResult(null);
            };

            using (var scope = new Gemini.Tests.TestInfrastructure.IoCOverrideScope(
                delegate(Type serviceType, string key)
                {
                    getInstanceCount++;
                    Assert.IsNull(serviceType);
                    Assert.IsNull(key);
                    return null;
                },
                delegate(Type serviceType)
                {
                    throw new InvalidOperationException(
                        "Unexpected multi-instance request for " + serviceType.FullName + ".");
                },
                delegate(object instance)
                {
                    throw new InvalidOperationException(
                        "Unexpected build-up request for " + instance.GetType().FullName + ".");
                }))
            using (ComposeShell(result, shell))
            {
                ((IResult)result).Execute(new CoroutineExecutionContext());
                await completionEvent.Task;
            }

            Assert.AreEqual(1, getInstanceCount);
            Assert.AreEqual(0, shell.OpenDocumentCallCount);
            Assert.IsNull(shell.OpenedDocument);
            Assert.AreEqual(1, completionCount);
            Assert.AreSame(result, completionSender);
            Assert.IsNotNull(completionArgs);
            Assert.IsNull(completionArgs.Error);
            Assert.IsTrue(completionArgs.WasCancelled);
        }

        private static System.ComponentModel.Composition.Hosting.CompositionContainer ComposeShell(
            OpenDocumentResult result,
            Gemini.Framework.Services.IShell shell,
            Gemini.Framework.Services.IEditorOpeningService editorOpeningService = null)
        {
            var container = new System.ComponentModel.Composition.Hosting.CompositionContainer();
            var batch = new System.ComponentModel.Composition.Hosting.CompositionBatch();
            System.ComponentModel.Composition.AttributedModelServices.AddExportedValue(batch, shell);
            editorOpeningService = editorOpeningService ?? new UnexpectedEditorOpeningService();
            System.ComponentModel.Composition.AttributedModelServices.AddExportedValue(
                batch,
                editorOpeningService);
            container.Compose(batch);
            System.ComponentModel.Composition.AttributedModelServices.SatisfyImportsOnce(container, result);
            return container;
        }

        private sealed class GatedEditorOpeningService :
            Gemini.Framework.Services.IEditorOpeningService
        {
            private readonly Gemini.Framework.IDocument _document;
            private readonly List<string> _markers;
            private readonly System.Threading.Tasks.TaskCompletionSource<Gemini.Framework.IDocument> _openGate;
            private readonly System.Threading.Tasks.TaskCompletionSource<object> _openEntered;

            public GatedEditorOpeningService(
                Gemini.Framework.IDocument document,
                List<string> markers)
            {
                _document = document;
                _markers = markers;
                _openGate = new System.Threading.Tasks.TaskCompletionSource<Gemini.Framework.IDocument>(
                    System.Threading.Tasks.TaskCreationOptions.RunContinuationsAsynchronously);
                _openEntered = new System.Threading.Tasks.TaskCompletionSource<object>(
                    System.Threading.Tasks.TaskCreationOptions.RunContinuationsAsynchronously);
            }

            public string OpenedPath { get; private set; }

            public System.Threading.Tasks.Task OpenEntered
            {
                get { return _openEntered.Task; }
            }

            public System.Threading.Tasks.Task<Gemini.Framework.IDocument> OpenFileAsync(
                string path,
                Action<Gemini.Framework.IDocument> configure = null)
            {
                OpenedPath = path;
                configure?.Invoke(_document);
                _markers.Add("EditorOperation");
                _openEntered.SetResult(null);
                return _openGate.Task;
            }

            public System.Threading.Tasks.Task<Gemini.Framework.IDocument> NewFileAsync(
                Gemini.Framework.Services.IEditorProvider editorProvider,
                string name)
            {
                throw new InvalidOperationException("Unexpected new editor call.");
            }

            public void CompleteOpen()
            {
                _openGate.SetResult(_document);
            }
        }

        private sealed class UnexpectedEditorOpeningService :
            Gemini.Framework.Services.IEditorOpeningService
        {
            public System.Threading.Tasks.Task<Gemini.Framework.IDocument> OpenFileAsync(
                string path,
                Action<Gemini.Framework.IDocument> configure = null)
            {
                throw new InvalidOperationException("Unexpected editor opening service call.");
            }

            public System.Threading.Tasks.Task<Gemini.Framework.IDocument> NewFileAsync(
                Gemini.Framework.Services.IEditorProvider editorProvider,
                string name)
            {
                throw new InvalidOperationException("Unexpected editor opening service call.");
            }
        }

        private sealed class TestDocument : Gemini.Framework.Document
        {
        }

        private sealed class GatedShell : Screen, Gemini.Framework.Services.IShell
        {
            private readonly List<string> _markers;
            private readonly System.Threading.Tasks.TaskCompletionSource<object> _openEntered;
            private readonly System.Threading.Tasks.TaskCompletionSource<object> _openGate;

            public GatedShell(List<string> markers)
            {
                _markers = markers;
                _openEntered = new System.Threading.Tasks.TaskCompletionSource<object>(
                    System.Threading.Tasks.TaskCreationOptions.RunContinuationsAsynchronously);
                _openGate = new System.Threading.Tasks.TaskCompletionSource<object>(
                    System.Threading.Tasks.TaskCreationOptions.RunContinuationsAsynchronously);
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

            public Gemini.Framework.ILayoutItem ActiveLayoutItem { get; set; }

            public Gemini.Framework.IDocument ActiveItem
            {
                get { return ActiveLayoutItem as Gemini.Framework.IDocument; }
            }

            public IObservableCollection<Gemini.Framework.IDocument> Documents
            {
                get { return null; }
            }

            public IObservableCollection<Gemini.Framework.ITool> Tools
            {
                get { return null; }
            }

            public int OpenDocumentCallCount { get; private set; }

            public Gemini.Framework.IDocument OpenedDocument { get; private set; }

            public System.Threading.Tasks.Task OpenEntered
            {
                get { return _openEntered.Task; }
            }

            public System.Threading.Tasks.Task InitializationTask
            {
                get { return System.Threading.Tasks.Task.CompletedTask; }
            }

            public bool RegisterTool(Gemini.Framework.ITool tool)
            {
                return false;
            }

            public System.Threading.Tasks.Task ShowToolAsync<TTool>()
                where TTool : Gemini.Framework.ITool
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }

            public System.Threading.Tasks.Task ShowToolAsync(Gemini.Framework.ITool model)
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }

            public System.Threading.Tasks.Task CloseToolAsync(Gemini.Framework.ITool tool)
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }

            public System.Threading.Tasks.Task OpenDocumentAsync(Gemini.Framework.IDocument model)
            {
                OpenDocumentCallCount++;
                OpenedDocument = model;
                _markers.Add("Open");
                _openEntered.SetResult(null);
                return _openGate.Task;
            }

            public System.Threading.Tasks.Task CloseDocumentAsync(Gemini.Framework.IDocument document)
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }

            public void Close()
            {
            }

            public void CompleteOpen()
            {
                _openGate.SetResult(null);
            }
        }
    }
}
