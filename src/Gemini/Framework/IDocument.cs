using Gemini.Modules.UndoRedo;

namespace Gemini.Framework
{
	public interface IDocument : ILayoutItem
	{
        IUndoRedoManager UndoRedoManager { get; }
	}
}