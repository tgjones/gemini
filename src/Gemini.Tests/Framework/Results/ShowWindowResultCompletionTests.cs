using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gemini.Tests.Framework.Results
{
    [TestClass]
    public class ShowWindowResultCompletionTests
    {
        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_ShowSuccess_CompletesOnlyWhenWindowCloses()
        {
            var showGate = new TaskCompletionSource<object>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            var window = new CompletionTestWindow();
            var manager = new ControlledWindowManager { ShowWindowTask = showGate.Task };
            var result = new ShowWindowResult<CompletionTestWindow>(window)
            {
                WindowManager = manager
            };
            var shutdownCount = 0;
            ((IOpenResult<CompletionTestWindow>)result).OnShutDown = delegate { shutdownCount++; };
            var capture = new CompletionCapture(result);

            ((IResult)result).Execute(new CoroutineExecutionContext());
            showGate.SetResult(null);
            Assert.IsFalse(capture.First.IsCompleted);

            await ResultTestLifecycle.CloseAsync(window);
            var completion = await capture.First;

            Assert.IsNull(completion.Error);
            Assert.IsFalse(completion.WasCancelled);
            Assert.AreEqual(1, shutdownCount);
            Assert.AreEqual(1, manager.ShowWindowCallCount);
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_ShowFault_CompletesWithErrorAndIgnoresLaterClose()
        {
            var error = new InvalidOperationException("Show failed.");
            var window = new CompletionTestWindow();
            var manager = new ControlledWindowManager
            {
                ShowWindowTask = Task.FromException(error)
            };
            var result = new ShowWindowResult<CompletionTestWindow>(window)
            {
                WindowManager = manager
            };
            var shutdownCount = 0;
            ((IOpenResult<CompletionTestWindow>)result).OnShutDown = delegate { shutdownCount++; };
            var capture = new CompletionCapture(result);

            ((IResult)result).Execute(new CoroutineExecutionContext());
            var completion = await capture.First;
            await ResultTestLifecycle.CloseAsync(window);

            Assert.AreSame(error, completion.Error);
            Assert.IsFalse(completion.WasCancelled);
            Assert.AreEqual(1, capture.Count);
            Assert.AreEqual(0, shutdownCount);
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_CanceledShow_CompletesAsCanceledAndIgnoresLaterClose()
        {
            var cancellation = new CancellationToken(true);
            var window = new CompletionTestWindow();
            var manager = new ControlledWindowManager
            {
                ShowWindowTask = Task.FromCanceled(cancellation)
            };
            var result = new ShowWindowResult<CompletionTestWindow>(window)
            {
                WindowManager = manager
            };
            var shutdownCount = 0;
            ((IOpenResult<CompletionTestWindow>)result).OnShutDown = delegate { shutdownCount++; };
            var capture = new CompletionCapture(result);

            ((IResult)result).Execute(new CoroutineExecutionContext());
            var completion = await capture.First;
            await ResultTestLifecycle.CloseAsync(window);

            Assert.IsNull(completion.Error);
            Assert.IsTrue(completion.WasCancelled);
            Assert.AreEqual(1, capture.Count);
            Assert.AreEqual(0, shutdownCount);
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_ConfigureThrows_CompletesWithErrorWithoutShowing()
        {
            var error = new InvalidOperationException("Configure failed.");
            var window = new CompletionTestWindow();
            var manager = new ControlledWindowManager();
            var result = new ShowWindowResult<CompletionTestWindow>(window)
            {
                WindowManager = manager
            };
            ((IOpenResult<CompletionTestWindow>)result).OnConfigure = delegate { throw error; };
            var capture = new CompletionCapture(result);

            ((IResult)result).Execute(new CoroutineExecutionContext());
            var completion = await capture.First;

            Assert.AreSame(error, completion.Error);
            Assert.AreEqual(0, manager.ShowWindowCallCount);
            Assert.AreEqual(1, capture.Count);
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_CompetingShowFaultAndClose_CompletesExactlyOnce()
        {
            var showGate = new TaskCompletionSource<object>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            var error = new InvalidOperationException("Show failed.");
            var window = new CompletionTestWindow();
            var manager = new ControlledWindowManager { ShowWindowTask = showGate.Task };
            var result = new ShowWindowResult<CompletionTestWindow>(window)
            {
                WindowManager = manager
            };
            var capture = new CompletionCapture(result);

            ((IResult)result).Execute(new CoroutineExecutionContext());
            await ((IActivate)window).ActivateAsync(CancellationToken.None);

            var closeTask = Task.Run(
                delegate { return ((IDeactivate)window).DeactivateAsync(true, CancellationToken.None); });
            var faultTask = Task.Run(delegate { showGate.SetException(error); });
            await Task.WhenAll(closeTask, faultTask);
            await capture.First;

            Assert.AreEqual(1, capture.Count);
            Assert.IsTrue(capture.Last.Error == null || ReferenceEquals(error, capture.Last.Error));
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_BackgroundShowFault_RaisesCompletedOnCapturedStaContext()
        {
            var executeThreadId = Environment.CurrentManagedThreadId;
            var synchronizationContext = new QueuedSynchronizationContext();
            var showGate = new TaskCompletionSource<object>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            var window = new CompletionTestWindow();
            var manager = new ControlledWindowManager { ShowWindowTask = showGate.Task };
            var result = new ShowWindowResult<CompletionTestWindow>(window)
            {
                WindowManager = manager
            };
            var capture = new CompletionCapture(result);

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

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_ReusedResult_DoesNotAccumulateCloseHandlers()
        {
            var window = new CompletionTestWindow();
            var manager = new ControlledWindowManager();
            var result = new ShowWindowResult<CompletionTestWindow>(window)
            {
                WindowManager = manager
            };
            var shutdownCount = 0;
            ((IOpenResult<CompletionTestWindow>)result).OnShutDown = delegate { shutdownCount++; };
            var capture = new CompletionCapture(result);

            ((IResult)result).Execute(new CoroutineExecutionContext());
            await ResultTestLifecycle.CloseAsync(window);
            await capture.First;

            ((IResult)result).Execute(new CoroutineExecutionContext());
            await ResultTestLifecycle.CloseAsync(window);
            await capture.Second;

            Assert.AreEqual(2, capture.Count);
            Assert.AreEqual(2, shutdownCount);
            Assert.AreEqual(2, manager.ShowWindowCallCount);
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_ShutdownThrows_CompletesWithShutdownErrorOnce()
        {
            var error = new InvalidOperationException("Shutdown failed.");
            var window = new CompletionTestWindow();
            var manager = new ControlledWindowManager();
            var result = new ShowWindowResult<CompletionTestWindow>(window)
            {
                WindowManager = manager
            };
            var shutdownCount = 0;
            ((IOpenResult<CompletionTestWindow>)result).OnShutDown = delegate
            {
                shutdownCount++;
                throw error;
            };
            var capture = new CompletionCapture(result);

            ((IResult)result).Execute(new CoroutineExecutionContext());
            await ResultTestLifecycle.CloseAsync(window);
            var completion = await capture.First;

            Assert.AreSame(error, completion.Error);
            Assert.IsFalse(completion.WasCancelled);
            Assert.AreEqual(1, shutdownCount);
            Assert.AreEqual(1, capture.Count);
        }
    }
}
