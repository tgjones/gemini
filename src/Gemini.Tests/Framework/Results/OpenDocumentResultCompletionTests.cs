using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework.Results;
using Gemini.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gemini.Tests.Framework.Results
{
    [TestClass]
    public class OpenDocumentResultCompletionTests
    {
        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_OpenSuccess_CompletesAfterOpenWithoutWaitingForClose()
        {
            var document = new CompletionTestDocument();
            var gate = new TaskCompletionSource<object>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            var shell = new ControlledShell { OpenDocumentTask = gate.Task };
            var result = new OpenDocumentResult(document);
            var capture = new CompletionCapture(result);

            using (ResultTestComposition.Compose(result, shell))
            {
                ((IResult)result).Execute(new CoroutineExecutionContext());
                Assert.IsFalse(capture.First.IsCompleted);

                gate.SetResult(null);
                var completion = await capture.First;

                Assert.IsNull(completion.Error);
                Assert.IsFalse(completion.WasCancelled);
                Assert.AreEqual(1, shell.OpenDocumentCallCount);
            }
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_OpenFault_ReportsOriginalErrorAndDetachesShutdown()
        {
            var error = new InvalidOperationException("Open failed.");
            var document = new CompletionTestDocument();
            var shell = new ControlledShell
            {
                OpenDocumentTask = Task.FromException(error)
            };
            var result = new OpenDocumentResult(document);
            var shutdownCount = 0;
            ((IOpenResult<Gemini.Framework.IDocument>)result).OnShutDown =
                delegate { shutdownCount++; };
            var capture = new CompletionCapture(result);

            using (ResultTestComposition.Compose(result, shell))
            {
                ((IResult)result).Execute(new CoroutineExecutionContext());
                var completion = await capture.First;
                await ResultTestLifecycle.CloseAsync(document);

                Assert.AreSame(error, completion.Error);
                Assert.IsFalse(completion.WasCancelled);
                Assert.AreEqual(1, capture.Count);
                Assert.AreEqual(0, shutdownCount);
            }
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_CanceledOpen_ReportsCancellationAndDetachesShutdown()
        {
            var cancellation = new CancellationToken(true);
            var document = new CompletionTestDocument();
            var shell = new ControlledShell
            {
                OpenDocumentTask = Task.FromCanceled(cancellation)
            };
            var result = new OpenDocumentResult(document);
            var shutdownCount = 0;
            ((IOpenResult<Gemini.Framework.IDocument>)result).OnShutDown =
                delegate { shutdownCount++; };
            var capture = new CompletionCapture(result);

            using (ResultTestComposition.Compose(result, shell))
            {
                ((IResult)result).Execute(new CoroutineExecutionContext());
                var completion = await capture.First;
                await ResultTestLifecycle.CloseAsync(document);

                Assert.IsNull(completion.Error);
                Assert.IsTrue(completion.WasCancelled);
                Assert.AreEqual(1, capture.Count);
                Assert.AreEqual(0, shutdownCount);
            }
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_ConfigureThrows_ReportsErrorWithoutOpening()
        {
            var error = new InvalidOperationException("Configure failed.");
            var document = new CompletionTestDocument();
            var shell = new ControlledShell();
            var result = new OpenDocumentResult(document);
            ((IOpenResult<Gemini.Framework.IDocument>)result).OnConfigure =
                delegate { throw error; };
            var capture = new CompletionCapture(result);

            using (ResultTestComposition.Compose(result, shell))
            {
                ((IResult)result).Execute(new CoroutineExecutionContext());
                var completion = await capture.First;

                Assert.AreSame(error, completion.Error);
                Assert.IsFalse(completion.WasCancelled);
                Assert.AreEqual(0, shell.OpenDocumentCallCount);
                Assert.AreEqual(1, capture.Count);
            }
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_BackgroundOpenCompletion_RaisesCompletedOnCapturedStaContext()
        {
            var executeThreadId = Environment.CurrentManagedThreadId;
            var synchronizationContext = new QueuedSynchronizationContext();
            var gate = new TaskCompletionSource<object>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            var document = new CompletionTestDocument();
            var shell = new ControlledShell { OpenDocumentTask = gate.Task };
            var result = new OpenDocumentResult(document);
            var capture = new CompletionCapture(result);

            using (ResultTestComposition.Compose(result, shell))
            {
                var previousContext = SynchronizationContext.Current;
                try
                {
                    SynchronizationContext.SetSynchronizationContext(synchronizationContext);
                    ((IResult)result).Execute(new CoroutineExecutionContext());
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(previousContext);
                }

                var backgroundCompletion = Task.Run(delegate { gate.SetResult(null); });
                synchronizationContext.Posted.GetAwaiter().GetResult();
                synchronizationContext.ExecuteNext();
                await backgroundCompletion;
                await capture.First;

                Assert.AreEqual(executeThreadId, capture.CallbackThreadId);
                Assert.AreEqual(1, synchronizationContext.PostCount);
            }
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_ReusedResult_DetachesOldCloseHandlerAndRunsShutdownOnce()
        {
            var document = new CompletionTestDocument();
            var shell = new ControlledShell();
            var result = new OpenDocumentResult(document);
            var shutdownCount = 0;
            ((IOpenResult<Gemini.Framework.IDocument>)result).OnShutDown =
                delegate { shutdownCount++; };
            var capture = new CompletionCapture(result);

            using (ResultTestComposition.Compose(result, shell))
            {
                ((IResult)result).Execute(new CoroutineExecutionContext());
                await capture.First;
                ((IResult)result).Execute(new CoroutineExecutionContext());
                await capture.Second;
                await ResultTestLifecycle.CloseAsync(document);

                Assert.AreEqual(2, capture.Count);
                Assert.AreEqual(1, shutdownCount);
                Assert.AreEqual(2, shell.OpenDocumentCallCount);
            }
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_CloseBeforeOpenCompletion_DoesNotChangeOpenLifetime()
        {
            var gate = new TaskCompletionSource<object>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            var document = new CompletionTestDocument();
            var shell = new ControlledShell { OpenDocumentTask = gate.Task };
            var result = new OpenDocumentResult(document);
            var shutdownCount = 0;
            ((IOpenResult<Gemini.Framework.IDocument>)result).OnShutDown =
                delegate { shutdownCount++; };
            var capture = new CompletionCapture(result);

            using (ResultTestComposition.Compose(result, shell))
            {
                ((IResult)result).Execute(new CoroutineExecutionContext());
                await ResultTestLifecycle.CloseAsync(document);
                Assert.AreEqual(0, capture.Count);
                Assert.AreEqual(1, shutdownCount);

                gate.SetResult(null);
                var completion = await capture.First;

                Assert.IsNull(completion.Error);
                Assert.IsFalse(completion.WasCancelled);
                Assert.AreEqual(1, capture.Count);
            }
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_ShutdownThrows_FaultsTheCloseLifecycleOnce()
        {
            var error = new InvalidOperationException("Shutdown failed.");
            var document = new CompletionTestDocument();
            var shell = new ControlledShell();
            var result = new OpenDocumentResult(document);
            var shutdownCount = 0;
            ((IOpenResult<Gemini.Framework.IDocument>)result).OnShutDown =
                delegate
                {
                    shutdownCount++;
                    throw error;
                };
            var capture = new CompletionCapture(result);

            using (ResultTestComposition.Compose(result, shell))
            {
                ((IResult)result).Execute(new CoroutineExecutionContext());
                await capture.First;

                var thrown = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                    delegate { return ResultTestLifecycle.CloseAsync(document); });

                Assert.AreSame(error, thrown);
                Assert.AreEqual(1, shutdownCount);
                Assert.AreEqual(1, capture.Count);
            }
        }
    }
}
