using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Gemini.Demo.Properties;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.Views;

namespace Gemini.Demo.Modules.Shell.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : Gemini.Modules.Shell.ViewModels.ShellViewModel
    {
        static ShellViewModel()
        {
            ViewLocator.AddNamespaceMapping(typeof(ShellViewModel).Namespace, typeof(ShellView).Namespace);
        }

        public override Task<bool> CanCloseAsync(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();

            Coroutine.BeginExecute(CanClose().GetEnumerator(), null, (s, e) => tcs.SetResult(!e.WasCancelled));

            return tcs.Task;
        }

        private IEnumerable<IResult> CanClose()
        {
            yield return new MessageBoxResult();
        }

        private class MessageBoxResult : IResult
        {
            public event EventHandler<ResultCompletionEventArgs> Completed;

            public void Execute(CoroutineExecutionContext context)
            {
                var result = System.Windows.MessageBoxResult.Yes;

                if (Settings.Default.ConfirmExit)
                {
                    result = MessageBox.Show("Are you sure you want to exit?", "Confirm", MessageBoxButton.YesNo);
                }

                Completed(this, new ResultCompletionEventArgs { WasCancelled = (result != System.Windows.MessageBoxResult.Yes) });
            }
        }
    }
}
