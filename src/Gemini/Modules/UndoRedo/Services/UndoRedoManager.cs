using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Collections;

namespace Gemini.Modules.UndoRedo.Services
{
    [Export(typeof(IUndoRedoManager))]
    public class UndoRedoManager : IUndoRedoManager
    {
        private readonly ObservableStack<IUndoableAction> _undoStack;
        private readonly ObservableStack<IUndoableAction> _redoStack;

        public IObservableCollection<IUndoableAction> UndoStack
        {
            get { return _undoStack; }
        }

        public IObservableCollection<IUndoableAction> RedoStack
        {
            get { return _redoStack; }
        }

        public UndoRedoManager()
        {
            _undoStack = new ObservableStack<IUndoableAction>();
            _redoStack = new ObservableStack<IUndoableAction>();
        }

        public void ExecuteAction(IUndoableAction action)
        {
            action.Execute();
            _undoStack.Push(action);
            _redoStack.Clear();
        }

        public void Undo(int actionCount)
        {
            for (int i = 0; i < actionCount; i++)
            {
                var action = _undoStack.Pop();
                action.Undo();
                _redoStack.Push(action);
            }
        }

        public void Redo(int actionCount)
        {
            for (int i = 0; i < actionCount; i++)
            {
                var action = _redoStack.Pop();
                action.Execute();
                _undoStack.Push(action);
            }
        }
    }
}