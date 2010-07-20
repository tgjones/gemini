using System;
using System.Windows.Input;
using Gemini.Contracts.Gui.Controls;

namespace Gemini.Contracts.Gui.Ribbon.RibbonButton
{
	public abstract class AbstractRibbonButton : AbstractButton, IRibbonButton
	{
		public string SizeDefinition { get; set; }

		protected AbstractRibbonButton()
		{
			SizeDefinition = "Middle,Small";
		}
	}

	public class RibbonButton : AbstractRibbonButton
	{
		private readonly ICommand _command;
		private readonly Func<object> _createCommandCallback;

		public RibbonButton(ICommand command, Func<object> createCommandCallback, string text)
		{
			_command = command;
			_createCommandCallback = createCommandCallback;
			Text = text;
		}

		protected override ICommand CreateCommand()
		{
			return _command;
		}

		protected override object CreateCommandParameter()
		{
			return _createCommandCallback();
		}
	}
}