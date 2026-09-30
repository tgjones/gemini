using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Gemini.Framework.Results
{
    public class ShowDialogResult<TWindow> : OpenResultBase<TWindow>
        where TWindow : IWindow
    {
        private readonly Func<TWindow> _windowLocator = () => IoC.Get<TWindow>();

        public ShowDialogResult()
        {
        }

        public ShowDialogResult(TWindow window)
        {
            _windowLocator = () => window;
        }

        [Import]
        public IWindowManager WindowManager { get; set; }

        public override void Execute(CoroutineExecutionContext context)
        {
            var execution = BeginExecution();

            try
            {
                var window = _windowLocator();
                _setData?.Invoke(window);
                _onConfigure?.Invoke(window);

                var dialogTask = WindowManager.ShowDialogAsync(window);
                _ = ObserveDialogAsync(execution, dialogTask, window);
            }
            catch (OperationCanceledException)
            {
                execution.TryComplete(null, true);
            }
            catch (Exception exception)
            {
                execution.TryComplete(exception, false);
            }
        }

        private async Task ObserveDialogAsync(
            ResultExecution execution,
            Task<bool?> dialogTask,
            TWindow window)
        {
            bool? result;
            try
            {
                result = await dialogTask.ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (dialogTask.IsCanceled)
                    execution.TryComplete(null, true);
                else
                    execution.TryComplete(exception, false);

                return;
            }

            execution.TryComplete(
                null,
                !result.GetValueOrDefault(),
                delegate { _onShutDown?.Invoke(window); });
        }
    }
}
