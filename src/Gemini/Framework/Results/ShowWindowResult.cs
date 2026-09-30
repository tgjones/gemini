using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Gemini.Framework.Results
{
    public class ShowWindowResult<TWindow> : OpenResultBase<TWindow>
        where TWindow : IWindow
    {
        private readonly Func<TWindow> _windowLocator = () => IoC.Get<TWindow>();

        [Import]
        public IWindowManager WindowManager { get; set; }

        public ShowWindowResult()
        {
        }

        public ShowWindowResult(TWindow window)
        {
            _windowLocator = () => window;
        }

        public override void Execute(CoroutineExecutionContext context)
        {
            var execution = BeginExecution();

            try
            {
                var window = _windowLocator();
                _setData?.Invoke(window);
                _onConfigure?.Invoke(window);

                execution.RegisterCleanup(SubscribeToClosed(window, window, execution));
                var showTask = WindowManager.ShowWindowAsync(window);
                _ = ObserveTaskAsync(execution, showTask, false);
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
    }
}
