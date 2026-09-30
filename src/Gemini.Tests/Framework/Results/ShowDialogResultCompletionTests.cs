using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gemini.Tests.Framework.Results
{
    [TestClass]
    public class ShowDialogResultCompletionTests
    {
        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_TrueResult_CompletesSuccessfullyAndRunsShutdownOnce()
        {
            var window = new CompletionTestWindow();
            var manager = new ControlledWindowManager
            {
                ShowDialogTask = Task.FromResult<bool?>(true)
            };
            var result = new ShowDialogResult<CompletionTestWindow>(window)
            {
                WindowManager = manager
            };
            var shutdownCount = 0;
            ((IOpenResult<CompletionTestWindow>)result).OnShutDown = delegate { shutdownCount++; };
            var capture = new CompletionCapture(result);

            ((IResult)result).Execute(new CoroutineExecutionContext());
            var completion = await capture.First;

            Assert.IsNull(completion.Error);
            Assert.IsFalse(completion.WasCancelled);
            Assert.AreEqual(1, shutdownCount);
            Assert.AreEqual(1, manager.ShowDialogCallCount);
            Assert.AreEqual(1, capture.Count);
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_FalseResult_MapsToUserCancellation()
        {
            var window = new CompletionTestWindow();
            var manager = new ControlledWindowManager
            {
                ShowDialogTask = Task.FromResult<bool?>(false)
            };
            var result = new ShowDialogResult<CompletionTestWindow>(window)
            {
                WindowManager = manager
            };
            var shutdownCount = 0;
            ((IOpenResult<CompletionTestWindow>)result).OnShutDown = delegate { shutdownCount++; };
            var capture = new CompletionCapture(result);

            ((IResult)result).Execute(new CoroutineExecutionContext());
            var completion = await capture.First;

            Assert.IsNull(completion.Error);
            Assert.IsTrue(completion.WasCancelled);
            Assert.AreEqual(1, shutdownCount);
            Assert.AreEqual(1, capture.Count);
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_NullResult_MapsToUserCancellation()
        {
            var window = new CompletionTestWindow();
            var manager = new ControlledWindowManager
            {
                ShowDialogTask = Task.FromResult<bool?>(null)
            };
            var result = new ShowDialogResult<CompletionTestWindow>(window)
            {
                WindowManager = manager
            };
            var shutdownCount = 0;
            ((IOpenResult<CompletionTestWindow>)result).OnShutDown = delegate { shutdownCount++; };
            var capture = new CompletionCapture(result);

            ((IResult)result).Execute(new CoroutineExecutionContext());
            var completion = await capture.First;

            Assert.IsNull(completion.Error);
            Assert.IsTrue(completion.WasCancelled);
            Assert.AreEqual(1, shutdownCount);
            Assert.AreEqual(1, capture.Count);
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_CanceledDialogTask_MapsToCancellationWithoutShutdown()
        {
            var cancellation = new CancellationToken(true);
            var window = new CompletionTestWindow();
            var manager = new ControlledWindowManager
            {
                ShowDialogTask = Task.FromCanceled<bool?>(cancellation)
            };
            var result = new ShowDialogResult<CompletionTestWindow>(window)
            {
                WindowManager = manager
            };
            var shutdownCount = 0;
            ((IOpenResult<CompletionTestWindow>)result).OnShutDown = delegate { shutdownCount++; };
            var capture = new CompletionCapture(result);

            ((IResult)result).Execute(new CoroutineExecutionContext());
            var completion = await capture.First;

            Assert.IsNull(completion.Error);
            Assert.IsTrue(completion.WasCancelled);
            Assert.AreEqual(0, shutdownCount);
            Assert.AreEqual(1, capture.Count);
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_FaultedDialogTaskWithCancellationException_MapsToError()
        {
            var error = new OperationCanceledException("Faulted, not canceled.");
            var window = new CompletionTestWindow();
            var manager = new ControlledWindowManager
            {
                ShowDialogTask = Task.FromException<bool?>(error)
            };
            var result = new ShowDialogResult<CompletionTestWindow>(window)
            {
                WindowManager = manager
            };
            var shutdownCount = 0;
            ((IOpenResult<CompletionTestWindow>)result).OnShutDown = delegate { shutdownCount++; };
            var capture = new CompletionCapture(result);

            ((IResult)result).Execute(new CoroutineExecutionContext());
            var completion = await capture.First;

            Assert.AreSame(error, completion.Error);
            Assert.IsFalse(completion.WasCancelled);
            Assert.AreEqual(0, shutdownCount);
            Assert.AreEqual(1, capture.Count);
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_ConfigureThrows_CompletesWithErrorWithoutShowing()
        {
            var error = new InvalidOperationException("Configure failed.");
            var window = new CompletionTestWindow();
            var manager = new ControlledWindowManager();
            var result = new ShowDialogResult<CompletionTestWindow>(window)
            {
                WindowManager = manager
            };
            ((IOpenResult<CompletionTestWindow>)result).OnConfigure = delegate { throw error; };
            var capture = new CompletionCapture(result);

            ((IResult)result).Execute(new CoroutineExecutionContext());
            var completion = await capture.First;

            Assert.AreSame(error, completion.Error);
            Assert.IsFalse(completion.WasCancelled);
            Assert.AreEqual(0, manager.ShowDialogCallCount);
            Assert.AreEqual(1, capture.Count);
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_BackgroundDialogCompletion_RaisesCompletedOnCapturedStaContext()
        {
            var executeThreadId = Environment.CurrentManagedThreadId;
            var synchronizationContext = new QueuedSynchronizationContext();
            var dialogGate = new TaskCompletionSource<bool?>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            var window = new CompletionTestWindow();
            var manager = new ControlledWindowManager { ShowDialogTask = dialogGate.Task };
            var result = new ShowDialogResult<CompletionTestWindow>(window)
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

            var backgroundCompletion = Task.Run(delegate { dialogGate.SetResult(true); });
            synchronizationContext.Posted.GetAwaiter().GetResult();
            synchronizationContext.ExecuteNext();
            await backgroundCompletion;
            await capture.First;

            Assert.AreEqual(executeThreadId, capture.CallbackThreadId);
            Assert.AreEqual(1, synchronizationContext.PostCount);
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_ReexecutedBeforeReturn_CancelsPriorExecutionAndIgnoresItsLateResult()
        {
            var firstDialog = new TaskCompletionSource<bool?>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            var window = new CompletionTestWindow();
            var manager = new ControlledWindowManager { ShowDialogTask = firstDialog.Task };
            var result = new ShowDialogResult<CompletionTestWindow>(window)
            {
                WindowManager = manager
            };
            var shutdownCount = 0;
            ((IOpenResult<CompletionTestWindow>)result).OnShutDown = delegate { shutdownCount++; };
            var capture = new CompletionCapture(result);

            ((IResult)result).Execute(new CoroutineExecutionContext());
            manager.ShowDialogTask = Task.FromResult<bool?>(true);
            ((IResult)result).Execute(new CoroutineExecutionContext());

            var firstCompletion = await capture.First;
            var secondCompletion = await capture.Second;
            firstDialog.SetResult(false);

            Assert.IsTrue(firstCompletion.WasCancelled);
            Assert.IsNull(firstCompletion.Error);
            Assert.IsFalse(secondCompletion.WasCancelled);
            Assert.IsNull(secondCompletion.Error);
            Assert.AreEqual(2, capture.Count);
            Assert.AreEqual(1, shutdownCount);
            Assert.AreEqual(2, manager.ShowDialogCallCount);
        }

        [STATestMethod]
        [Timeout(10000)]
        public async Task Execute_ShutdownThrows_CompletesWithShutdownErrorOnce()
        {
            var error = new InvalidOperationException("Shutdown failed.");
            var window = new CompletionTestWindow();
            var manager = new ControlledWindowManager();
            var result = new ShowDialogResult<CompletionTestWindow>(window)
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
            var completion = await capture.First;

            Assert.AreSame(error, completion.Error);
            Assert.IsFalse(completion.WasCancelled);
            Assert.AreEqual(1, shutdownCount);
            Assert.AreEqual(1, capture.Count);
        }
    }
}
