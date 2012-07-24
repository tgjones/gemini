using System.Windows.Input;
using Caliburn.Micro;

namespace Gemini.Framework
{
	public interface IDocument : IScreen
	{
		ICommand CloseCommand { get; }
	}
}