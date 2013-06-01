using Gemini.Framework;

namespace Gemini.Modules.UndoRedo
{
    public interface IHistoryTool : ITool
    {
        IUndoRedoManager UndoRedoManager { get; set; }
    }
}