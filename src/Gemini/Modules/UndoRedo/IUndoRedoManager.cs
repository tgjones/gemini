using System;
using Caliburn.Micro;

namespace Gemini.Modules.UndoRedo
{
    public interface IUndoRedoManager
    {
        IObservableCollection<IUndoableAction> UndoStack { get; }
        IObservableCollection<IUndoableAction> RedoStack { get; }

        event EventHandler BatchBegin;
        event EventHandler BatchEnd;

        int? UndoCountLimit { get; set; }

        void ExecuteAction(IUndoableAction action);

        void Undo(int actionCount);
        void UndoTo(IUndoableAction action);
        void UndoAll();

        void Redo(int actionCount);
        void RedoTo(IUndoableAction action);
    }
}