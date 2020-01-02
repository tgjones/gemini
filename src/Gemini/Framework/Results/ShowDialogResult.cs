using System;
using System.ComponentModel.Composition;
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
            var window = _windowLocator();

            _setData?.Invoke(window);

            _onConfigure?.Invoke(window);

            WindowManager
                .ShowDialogAsync(window)
                .ContinueWith(t =>
                {
                    var result = t.Result.GetValueOrDefault();

                    _onShutDown?.Invoke(window);

                    OnCompleted(null, !result);
                });
        }
    }
}
