using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Modules.UndoRedo;

namespace Gemini.Framework
{
	public interface IDocument : IScreen
	{
        IUndoRedoManager UndoRedoManager { get; }
		ICommand CloseCommand { get; }
	}
}