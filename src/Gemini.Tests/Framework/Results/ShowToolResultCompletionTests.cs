using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gemini.Tests.Framework.Results
{
    [TestClass]
    public class ShowToolResultCompletionTests
    {
        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_ShowSuccess_CompletesOnlyWhenToolCloses()
        {
            var showGate = new TaskCompletionSource<object>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            var tool = new CompletionTestTool();
            var shell = new ControlledShell { ShowToolTask = showGate.Task };
            var result = new ShowToolResult<CompletionTestTool>(tool);
            var shutdownCount = 0;
            ((IOpenResult<CompletionTestTool>)result).OnShutDown = delegate { shutdownCount++; };
            var capture = new CompletionCapture(result);

            using (ResultTestComposition.Compose(result, shell))
            {
                ((IResult)result).Execute(new CoroutineExecutionContext());
                showGate.SetResult(null);
                Assert.IsFalse(capture.First.IsCompleted);

                await ResultTestLifecycle.CloseAsync(tool);
                var completion = await capture.First;

                Assert.IsNull(completion.Error);
                Assert.IsFalse(completion.WasCancelled);
                Assert.AreEqual(1, shutdownCount);
                Assert.AreEqual(1, shell.ShowToolCallCount);
            }
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_ShowFault_CompletesWithErrorAndIgnoresLaterClose()
        {
            var error = new InvalidOperationException("Show failed.");
            var tool = new CompletionTestTool();
            var shell = new ControlledShell { ShowToolTask = Task.FromException(error) };
            var result = new ShowToolResult<CompletionTestTool>(tool);
            var shutdownCount = 0;
            ((IOpenResult<CompletionTestTool>)result).OnShutDown = delegate { shutdownCount++; };
            var capture = new CompletionCapture(result);

            using (ResultTestComposition.Compose(result, shell))
            {
                ((IResult)result).Execute(new CoroutineExecutionContext());
                var completion = await capture.First;
                await ResultTestLifecycle.CloseAsync(tool);

                Assert.AreSame(error, completion.Error);
                Assert.IsFalse(completion.WasCancelled);
                Assert.AreEqual(1, capture.Count);
                Assert.AreEqual(0, shutdownCount);
            }
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_CanceledShow_CompletesAsCanceledAndIgnoresLaterClose()
        {
            var cancellation = new CancellationToken(true);
            var tool = new CompletionTestTool();
            var shell = new ControlledShell
            {
                ShowToolTask = Task.FromCanceled(cancellation)
            };
            var result = new ShowToolResult<CompletionTestTool>(tool);
            var shutdownCount = 0;
            ((IOpenResult<CompletionTestTool>)result).OnShutDown = delegate { shutdownCount++; };
            var capture = new CompletionCapture(result);

            using (ResultTestComposition.Compose(result, shell))
            {
                ((IResult)result).Execute(new CoroutineExecutionContext());
                var completion = await capture.First;
                await ResultTestLifecycle.CloseAsync(tool);

                Assert.IsNull(completion.Error);
                Assert.IsTrue(completion.WasCancelled);
                Assert.AreEqual(1, capture.Count);
                Assert.AreEqual(0, shutdownCount);
            }
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_ConfigureThrows_CompletesWithErrorWithoutShowing()
        {
            var error = new InvalidOperationException("Configure failed.");
            var tool = new CompletionTestTool();
            var shell = new ControlledShell();
            var result = new ShowToolResult<CompletionTestTool>(tool);
            ((IOpenResult<CompletionTestTool>)result).OnConfigure = delegate { throw error; };
            var capture = new CompletionCapture(result);

            using (ResultTestComposition.Compose(result, shell))
            {
                ((IResult)result).Execute(new CoroutineExecutionContext());
                var completion = await capture.First;

                Assert.AreSame(error, completion.Error);
                Assert.AreEqual(0, shell.ShowToolCallCount);
                Assert.AreEqual(1, capture.Count);
            }
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_CompetingShowFaultAndClose_CompletesExactlyOnce()
        {
            var showGate = new TaskCompletionSource<object>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            var error = new InvalidOperationException("Show failed.");
            var tool = new CompletionTestTool();
            var shell = new ControlledShell { ShowToolTask = showGate.Task };
            var result = new ShowToolResult<CompletionTestTool>(tool);
            var capture = new CompletionCapture(result);

            using (ResultTestComposition.Compose(result, shell))
            {
                ((IResult)result).Execute(new CoroutineExecutionContext());
                await ((IActivate)tool).ActivateAsync(CancellationToken.None);

                var closeTask = Task.Run(
                    delegate { return ((IDeactivate)tool).DeactivateAsync(true, CancellationToken.None); });
                var faultTask = Task.Run(delegate { showGate.SetException(error); });
                await Task.WhenAll(closeTask, faultTask);
                await capture.First;

                Assert.AreEqual(1, capture.Count);
                Assert.IsTrue(capture.Last.Error == null || ReferenceEquals(error, capture.Last.Error));
            }
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_BackgroundShowFault_RaisesCompletedOnCapturedStaContext()
        {
            var executeThreadId = Environment.CurrentManagedThreadId;
            var synchronizationContext = new QueuedSynchronizationContext();
            var showGate = new TaskCompletionSource<object>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            var tool = new CompletionTestTool();
            var shell = new ControlledShell { ShowToolTask = showGate.Task };
            var result = new ShowToolResult<CompletionTestTool>(tool);
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

                var backgroundCompletion = Task.Run(delegate
                {
                    showGate.SetException(new InvalidOperationException("Show failed."));
                });
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
        public async Task Execute_ReusedResult_DoesNotAccumulateCloseHandlers()
        {
            var tool = new CompletionTestTool();
            var shell = new ControlledShell();
            var result = new ShowToolResult<CompletionTestTool>(tool);
            var shutdownCount = 0;
            ((IOpenResult<CompletionTestTool>)result).OnShutDown = delegate { shutdownCount++; };
            var capture = new CompletionCapture(result);

            using (ResultTestComposition.Compose(result, shell))
            {
                ((IResult)result).Execute(new CoroutineExecutionContext());
                await ResultTestLifecycle.CloseAsync(tool);
                await capture.First;

                ((IResult)result).Execute(new CoroutineExecutionContext());
                await ResultTestLifecycle.CloseAsync(tool);
                await capture.Second;

                Assert.AreEqual(2, capture.Count);
                Assert.AreEqual(2, shutdownCount);
                Assert.AreEqual(2, shell.ShowToolCallCount);
            }
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_ShutdownThrows_CompletesWithShutdownErrorOnce()
        {
            var error = new InvalidOperationException("Shutdown failed.");
            var tool = new CompletionTestTool();
            var shell = new ControlledShell();
            var result = new ShowToolResult<CompletionTestTool>(tool);
            var shutdownCount = 0;
            ((IOpenResult<CompletionTestTool>)result).OnShutDown = delegate
            {
                shutdownCount++;
                throw error;
            };
            var capture = new CompletionCapture(result);

            using (ResultTestComposition.Compose(result, shell))
            {
                ((IResult)result).Execute(new CoroutineExecutionContext());
                await ResultTestLifecycle.CloseAsync(tool);
                var completion = await capture.First;

                Assert.AreSame(error, completion.Error);
                Assert.IsFalse(completion.WasCancelled);
                Assert.AreEqual(1, shutdownCount);
                Assert.AreEqual(1, capture.Count);
            }
        }
    }
}
