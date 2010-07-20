using System.Windows.Input;

namespace Gemini.Contracts.Gui.Controls
{
	public abstract class AbstractCommandControl : AbstractControl, ICommandControl
	{
		private ICommand _command;
		private object _commandParameter;

		public ICommand Command
		{
			get { return _command ?? (_command = CreateCommand()); }
		}

		public object CommandParameter
		{
			get { return _commandParameter ?? (_commandParameter = CreateCommandParameter()); }
		}

		protected abstract ICommand CreateCommand();

		protected virtual object CreateCommandParameter()
		{
			return null;
		}
	}
}