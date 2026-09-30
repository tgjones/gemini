using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Gemini.Framework.Results
{
    /// <summary>
    /// Coordinates one terminal completion for each result execution on the
    /// synchronization context captured when execution begins.
    /// </summary>
    public abstract class OpenResultBase<TTarget> : IOpenResult<TTarget>
    {
        protected Action<TTarget> _setData;
        protected Action<TTarget> _onConfigure;
        protected Action<TTarget> _onShutDown;

        private ResultExecution _currentExecution;

        Action<TTarget> IOpenResult<TTarget>.OnConfigure
        {
            get { return _onConfigure; }
            set { _onConfigure = value; }
        }

        Action<TTarget> IOpenResult<TTarget>.OnShutDown
        {
            get { return _onShutDown; }
            set { _onShutDown = value; }
        }

        /// <summary>
        /// Starts a new execution and cancels any unfinished prior execution.
        /// </summary>
        protected ResultExecution BeginExecution()
        {
            var execution = new ResultExecution(this, SynchronizationContext.Current);
            var previousExecution = Interlocked.Exchange(ref _currentExecution, execution);
            previousExecution?.TryComplete(null, true);
            return execution;
        }

        protected virtual void OnCompleted(Exception exception, bool wasCancelled)
        {
            var execution = Volatile.Read(ref _currentExecution);
            if (execution == null)
                execution = BeginExecution();

            execution.TryComplete(exception, wasCancelled);
        }

        protected async Task ObserveTaskAsync(
            ResultExecution execution,
            Task task,
            bool completeOnSuccess,
            System.Action onSuccess = null,
            System.Action onFailure = null)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (task.IsCanceled)
                    execution.TryComplete(null, true, onFailure);
                else
                    execution.TryComplete(exception, false, onFailure);

                return;
            }

            if (completeOnSuccess)
                execution.TryComplete(null, false, onSuccess);
        }

        protected System.Action SubscribeToClosed(
            TTarget target,
            IDeactivate deactivatable,
            ResultExecution execution)
        {
            var isDetached = 0;
            AsyncEventHandler<DeactivationEventArgs> handler = null;
            handler = delegate(object sender, DeactivationEventArgs eventArgs)
            {
                if (!eventArgs.WasClosed || Interlocked.Exchange(ref isDetached, 1) != 0)
                    return Task.CompletedTask;

                deactivatable.Deactivated -= handler;

                if (execution != null)
                {
                    execution.TryComplete(
                        null,
                        false,
                        delegate { _onShutDown?.Invoke(target); });
                    return Task.CompletedTask;
                }

                try
                {
                    _onShutDown?.Invoke(target);
                    return Task.CompletedTask;
                }
                catch (Exception exception)
                {
                    return Task.FromException(exception);
                }
            };

            deactivatable.Deactivated += handler;

            return delegate
            {
                if (Interlocked.Exchange(ref isDetached, 1) == 0)
                    deactivatable.Deactivated -= handler;
            };
        }

        public abstract void Execute(CoroutineExecutionContext context);

        public event EventHandler<ResultCompletionEventArgs> Completed;

        private void RaiseCompleted(
            SynchronizationContext synchronizationContext,
            ResultCompletionEventArgs eventArgs)
        {
            var completedHandler = Completed;
            if (completedHandler == null)
                return;

            if (synchronizationContext == null ||
                ReferenceEquals(synchronizationContext, SynchronizationContext.Current))
            {
                completedHandler(this, eventArgs);
                return;
            }

            synchronizationContext.Post(
                delegate { completedHandler(this, eventArgs); },
                null);
        }

        protected sealed class ResultExecution
        {
            private readonly OpenResultBase<TTarget> _owner;
            private readonly SynchronizationContext _synchronizationContext;
            private System.Action _cleanup;
            private int _isCompleted;

            internal ResultExecution(
                OpenResultBase<TTarget> owner,
                SynchronizationContext synchronizationContext)
            {
                _owner = owner;
                _synchronizationContext = synchronizationContext;
            }

            public void RegisterCleanup(System.Action cleanup)
            {
                if (cleanup == null)
                    throw new ArgumentNullException(nameof(cleanup));

                if (Interlocked.CompareExchange(ref _cleanup, cleanup, null) != null)
                    throw new InvalidOperationException("A cleanup callback is already registered.");

                if (Volatile.Read(ref _isCompleted) != 0)
                    Interlocked.Exchange(ref _cleanup, null)?.Invoke();
            }

            public bool TryComplete(
                Exception exception,
                bool wasCancelled,
                System.Action terminalAction = null)
            {
                // The first terminal path owns cleanup and the terminal action.
                // Competing task and close outcomes are intentionally no-ops.
                if (Interlocked.Exchange(ref _isCompleted, 1) != 0)
                    return false;

                try
                {
                    Interlocked.Exchange(ref _cleanup, null)?.Invoke();
                    terminalAction?.Invoke();
                }
                catch (Exception terminalException)
                {
                    exception = terminalException;
                    wasCancelled = false;
                }

                _owner.RaiseCompleted(
                    _synchronizationContext,
                    new ResultCompletionEventArgs
                    {
                        Error = exception,
                        WasCancelled = wasCancelled
                    });
                return true;
            }
        }
    }
}