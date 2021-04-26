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
        [Import]
		private readonly IShell _shell;
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
			var tool = _toolLocator();

			if (_setData != null)
				_setData(tool);

			if (_onConfigure != null)
				_onConfigure(tool);

			tool.Deactivated += (s, e) =>
			{
                if (e.WasClosed)
                {
                    if (_onShutDown != null)
                        _onShutDown(tool);

                    OnCompleted(null, false);
                }

                return System.Threading.Tasks.Task.CompletedTask;
            };

			_shell.ShowTool(tool);
		}
	}
}
