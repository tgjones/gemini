using System.Windows.Input;
using Caliburn.Micro;

namespace Gemini.Framework
{
	public interface ITool : IScreen
	{
		ICommand CloseCommand { get; }
	}
}