using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace Gemini.Framework.Results
{
	public class ShowToolResult<TTool> : OpenResultBase<TTool>
		where TTool : ITool
	{
		private readonly PaneLocation _pane;
		private readonly Func<TTool> _toolLocator = () => IoC.Get<TTool>();

		[Import]
		private IShell _shell;

		public ShowToolResult(PaneLocation pane)
		{
			_pane = pane;
		}

		public ShowToolResult(PaneLocation pane, TTool tool)
		{
			_pane = pane;
			_toolLocator = () => tool;
		}

		public PaneLocation Pane
		{
			get { return _pane; }
		}

		public override void Execute(ActionExecutionContext context)
		{
			var tool = _toolLocator();

			if (_setData != null)
				_setData(tool);

			if (_onConfigure != null)
				_onConfigure(tool);

			tool.Deactivated += (s, e) =>
			{
				if (_onShutDown != null)
					_onShutDown(tool);

				OnCompleted(null);
			};

			_shell.ShowTool(_pane, tool);
		}
	}
}