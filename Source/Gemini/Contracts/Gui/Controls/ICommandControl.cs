using System.Windows.Input;

namespace Gemini.Contracts.Gui.Controls
{
	public interface ICommandControl : IControl
	{
		ICommand Command { get; }
	}
}