using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gemini.Modules.Shell.ViewModels
{
    /// <summary>
    /// Serializes independent shell lifecycle transitions in arrival order while
    /// allowing a transition to await nested work from its own asynchronous flow.
    /// </summary>
    internal sealed class TransitionCoordinator
    {
        private readonly object _syncRoot = new object();
        private readonly Queue<TransitionRequest> _requests = new Queue<TransitionRequest>();
        private readonly AsyncLocal<TransitionRequest> _flowRequest =
            new AsyncLocal<TransitionRequest>();
        private TransitionRequest _executingRequest;
        private bool _isProcessing;

        /// <summary>
        /// Queues <paramref name="transition"/> behind unrelated work, or executes it
        /// inline when the currently executing transition awaits the nested request.
        /// </summary>
        public Task Enqueue(Func<Task> transition)
        {
            if (transition == null)
                throw new ArgumentNullException(nameof(transition));

            var request = new TransitionRequest(transition);
            var startProcessing = false;
            var executeNested = false;

            lock (_syncRoot)
            {
                // A transition that awaits a nested shell request would deadlock if that
                // request were queued behind its parent. Other asynchronous flows remain FIFO.
                executeNested = _flowRequest.Value != null
                    && ReferenceEquals(_executingRequest, _flowRequest.Value);
                if (!executeNested)
                {
                    _requests.Enqueue(request);
                    if (!_isProcessing)
                    {
                        _isProcessing = true;
                        startProcessing = true;
                    }
                }
            }

            if (executeNested)
                return ExecuteNestedAsync(transition);

            if (startProcessing)
                _ = ProcessQueueAsync();

            return request.Completion.Task;
        }

        private async Task ProcessQueueAsync()
        {
            while (true)
            {
                TransitionRequest request;
                lock (_syncRoot)
                {
                    if (_requests.Count == 0)
                    {
                        _isProcessing = false;
                        return;
                    }

                    request = _requests.Dequeue();
                    _executingRequest = request;
                }

                var previousFlowRequest = _flowRequest.Value;
                _flowRequest.Value = request;
                try
                {
                    await request.Transition();
                    request.Completion.TrySetResult(null);
                }
                catch (OperationCanceledException)
                {
                    request.Completion.TrySetCanceled();
                }
                catch (Exception exception)
                {
                    request.Completion.TrySetException(exception);
                }
                finally
                {
                    _flowRequest.Value = previousFlowRequest;
                    lock (_syncRoot)
                    {
                        if (ReferenceEquals(_executingRequest, request))
                            _executingRequest = null;
                    }
                }
            }
        }

        private static async Task ExecuteNestedAsync(Func<Task> transition)
        {
            await transition();
        }

        private sealed class TransitionRequest
        {
            public TransitionRequest(Func<Task> transition)
            {
                Transition = transition;
                Completion = new TaskCompletionSource<object>(
                    TaskCreationOptions.RunContinuationsAsynchronously);
            }

            public Func<Task> Transition { get; }

            public TaskCompletionSource<object> Completion { get; }
        }
    }
}
