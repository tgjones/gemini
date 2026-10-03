using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace Gemini.Framework.Results
{
    public class ShowToolResult<TTool> : OpenResultBase<TTool>
        where TTool : ITool
    {
        private readonly Func<TTool> _toolLocator = () => IoC.Get<TTool>();

#pragma warning disable 649
#pragma warning disable IDE0044 // Add readonly modifier
        [Import] private IShell _shell;
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore 649

        public ShowToolResult()
        {
        }

        public ShowToolResult(TTool tool)
        {
            _toolLocator = () => tool;
        }

        public override void Execute(CoroutineExecutionContext context)
        {
            var execution = BeginExecution();

            try
            {
                var tool = _toolLocator();
                _setData?.Invoke(tool);
                _onConfigure?.Invoke(tool);

                execution.RegisterCleanup(SubscribeToClosed(tool, tool, execution));
                var showTask = _shell.ShowToolAsync(tool);
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
