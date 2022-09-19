using System;
using System.ComponentModel;
using Caliburn.Micro;

namespace Gemini.Modules.UndoRedo
{
    public interface IUndoRedoManager : INotifyPropertyChanged, IDisposable
    {
        IObservableCollection<IUndoableAction> ActionStack { get; }
        IUndoableAction CurrentAction { get; }
        int UndoActionCount { get; }
        int RedoActionCount { get; }

        event EventHandler BatchBegin;
        event EventHandler BatchEnd;

        int? UndoCountLimit { get; set; }

        void ExecuteAction(IUndoableAction action);
        void PushAction(IUndoableAction action);

        bool CanUndo { get; }
        void Undo(int actionCount);
        void UndoTo(IUndoableAction action);
        void UndoAll();

        bool CanRedo { get; }
        void Redo(int actionCount);
        void RedoTo(IUndoableAction action);
    }
}
