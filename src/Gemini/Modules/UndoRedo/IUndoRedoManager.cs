using Caliburn.Micro;

namespace Gemini.Modules.UndoRedo
{
    public interface IUndoRedoManager
    {
        IObservableCollection<IUndoableAction> UndoStack { get; }
        IObservableCollection<IUndoableAction> RedoStack { get; }

        void ExecuteAction(IUndoableAction action);

        void Undo(int actionCount);
        void Redo(int actionCount);
    }
}