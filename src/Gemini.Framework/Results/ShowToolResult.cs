using System;
using Gemini.Framework.Services;
using Microsoft.Practices.ServiceLocation;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;

namespace Gemini.Framework.Results
{
	public class ShowToolResult<TTool> : OpenResultBase<TTool>
				where TTool : IExtendedPresenter
	{
		private readonly Pane _pane;
		private readonly Func<TTool> _toolLocator = () => ServiceLocator.Current.GetInstance<TTool>();

		public ShowToolResult(Pane pane)
		{
			_pane = pane;
		}

		public ShowToolResult(Pane pane, TTool tool)
		{
			_pane = pane;
			_toolLocator = () => tool;
		}

		public Pane Pane
		{
			get { return _pane; }
		}

		public override void Execute(IRoutedMessageWithOutcome message, IInteractionNode handlingNode)
		{
			var shell = ServiceLocator.Current.GetInstance<IShell>();
			var tool = _toolLocator();

			if (_setData != null)
				_setData(tool);

			if (_onConfigure != null)
				_onConfigure(tool);

			tool.WasShutdown +=
					(s, e) =>
					{
						if (_onShutDown != null)
							_onShutDown(tool);

						OnCompleted(null);
					};

			shell.ShowTool(_pane, tool);
		}
	}
}