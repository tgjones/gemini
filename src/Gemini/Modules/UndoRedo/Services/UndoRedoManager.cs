using System;
using Caliburn.Micro;

namespace Gemini.Modules.UndoRedo.Services
{
    public class UndoRedoManager : IUndoRedoManager
    {
        private readonly BindableCollection<IUndoableAction> _undoStack;
        private readonly BindableCollection<IUndoableAction> _redoStack;

        public event EventHandler BatchBegin;
        public event EventHandler BatchEnd;

        public IObservableCollection<IUndoableAction> UndoStack
        {
            get { return _undoStack; }
        }

        public IObservableCollection<IUndoableAction> RedoStack
        {
            get { return _redoStack; }
        }

        private int? _undoCountLimit = null;

        public int? UndoCountLimit
        {
            get { return _undoCountLimit; }

            set
            {
                _undoCountLimit = value;
                EnforceLimit();
            }
        }

        private void EnforceLimit()
        {
            if (!_undoCountLimit.HasValue)
                return;

            while (_undoStack.Count > UndoCountLimit.Value)
                PopFront(_undoStack);
        }

        public UndoRedoManager()
        {
            _undoStack = new BindableCollection<IUndoableAction>();
            _redoStack = new BindableCollection<IUndoableAction>();
        }

        public void ExecuteAction(IUndoableAction action)
        {
            action.Execute();
            Push(_undoStack, action);
            _redoStack.Clear();
            EnforceLimit();
        }

        public void Undo(int actionCount)
        {
            OnBegin();

            try
            {
                for (int i = 0; i < actionCount; i++)
                {
                    var action = Pop(_undoStack);
                    action.Undo();
                    Push(_redoStack, action);
                }
            }
            finally
            {
                OnEnd();
            }
        }

        public void UndoTo(IUndoableAction action)
        {
            OnBegin();

            try
            {
                while (true)
                {
                    if (Peek(_undoStack) == action)
                        return;
                    var thisAction = Pop(_undoStack);
                    thisAction.Undo();
                    Push(_redoStack, thisAction);
                }
            }
            finally
            {
                OnEnd();
            }
        }

        public void UndoAll()
        {
            Undo(_undoStack.Count);
        }

        public void Redo(int actionCount)
        {
            OnBegin();

            try
            {
                for (int i = 0; i < actionCount; i++)
                {
                    var action = Pop(_redoStack);
                    action.Execute();
                    Push(_undoStack, action);
                }

                EnforceLimit();
            }
            finally
            {
                OnEnd();
            }
        }

        public void RedoTo(IUndoableAction action)
        {
            OnBegin();

            try
            {
                while (true)
                {
                    var thisAction = Pop(_redoStack);
                    thisAction.Execute();
                    Push(_undoStack, thisAction);
                    if (thisAction == action)
                        break;
                }

                EnforceLimit();
            }
            finally
            {
                OnEnd();
            }
        }

        private void OnBegin()
        {
            var handler = BatchBegin;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void OnEnd()
        {
            var handler = BatchEnd;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private static IUndoableAction Peek(BindableCollection<IUndoableAction> stack)
        {
            return stack[stack.Count - 1];
        }

        private static void Push(BindableCollection<IUndoableAction> stack, IUndoableAction action)
        {
            stack.Add(action);
        }

        private static IUndoableAction Pop(BindableCollection<IUndoableAction> stack)
        {
            var item = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            return item;
        }

        private static IUndoableAction PopFront(BindableCollection<IUndoableAction> stack)
        {
            var item = stack[0];
            stack.RemoveAt(0);
            return item;
        }
    }
}