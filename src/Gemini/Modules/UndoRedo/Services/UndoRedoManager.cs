using System;
using Caliburn.Micro;

namespace Gemini.Modules.UndoRedo.Services
{
    public class UndoRedoManager : PropertyChangedBase, IUndoRedoManager
    {
        public IObservableCollection<IUndoableAction> ActionStack { get; } = new BindableCollection<IUndoableAction>();

        public IUndoableAction CurrentAction => UndoActionCount > 0 ? ActionStack[UndoActionCount - 1] : null;

        public event EventHandler BatchBegin;
        public event EventHandler BatchEnd;

        private int _undoActionCount;

        public int UndoActionCount
        {
            get => _undoActionCount;

            private set
            {
                if (_undoActionCount == value)
                    return;

                _undoActionCount = value;

                NotifyOfPropertyChange(() => UndoActionCount);
                NotifyOfPropertyChange(() => RedoActionCount);
                NotifyOfPropertyChange(() => CanUndo);
                NotifyOfPropertyChange(() => CanRedo);
            }
        }

        private int? _undoCountLimit = null;

        public int RedoActionCount => ActionStack.Count - UndoActionCount;

        public int? UndoCountLimit
        {
            get => _undoCountLimit;

            set
            {
                _undoCountLimit = value;
                EnforceLimit();
            }
        }

        private void EnforceLimit()
        {
            if (!UndoCountLimit.HasValue)
                return;

            var removeCount = ActionStack.Count - UndoCountLimit.Value;
            if (removeCount <= 0)
                return;

            for (var i = removeCount - 1; i >= 0; i--)
                RemoveAction(i);
            UndoActionCount -= removeCount;
        }
        
        public void ExecuteAction(IUndoableAction action)
        {
            action.Execute();
            PushAction(action);
        }

        public void PushAction(IUndoableAction action)
        {
            if (UndoActionCount < ActionStack.Count)
            {
                // We currently have items that can be redone, remove those
                for (var i = ActionStack.Count - 1; i >= UndoActionCount; i--)
                    RemoveAction(i);

                NotifyOfPropertyChange(() => RedoActionCount);
                NotifyOfPropertyChange(() => CanRedo);
            }

            ActionStack.Add(action);
            UndoActionCount++;

            EnforceLimit();
        }

        public bool CanUndo => UndoActionCount > 0;

        public void Undo(int actionCount)
        {
            if (actionCount <= 0 || actionCount > UndoActionCount)
                throw new ArgumentOutOfRangeException(nameof(actionCount));

            OnBegin();

            try
            {
                for (var i = 1; i <= actionCount; i++)
                    ActionStack[UndoActionCount - i].Undo();

                UndoActionCount -= actionCount;
            }
            finally
            {
                OnEnd();
            }
        }

        public void UndoTo(IUndoableAction action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (UndoActionCount < 1)
                throw new InvalidOperationException();

            // Find the action first to prevent endless loops and to only update UndoActions once
            // Do the loop in reverse from the end of the undo actions to skip searching any redo actions
            var i = UndoActionCount - 1;
            for (; i >= 0; i--)
            {
                if (ActionStack[i] == action)
                    break;
            }

            if (i < 0)
                throw new InvalidOperationException();

            Undo(UndoActionCount - i - 1);
        }

        public void UndoAll()
        {
            if (UndoActionCount <= 0)
                return;

            OnBegin();

            for (var i = UndoActionCount - 1; i >= 0; i--)
                ActionStack[i].Undo();

            OnEnd();

            UndoActionCount = 0;
        }

        public bool CanRedo => RedoActionCount > 0;

        public void Redo(int actionCount)
        {
            if (actionCount <= 0 || actionCount > RedoActionCount)
                throw new ArgumentOutOfRangeException(nameof(actionCount));

            OnBegin();

            try
            {
                for (var i = 0; i < actionCount; i++)
                    ActionStack[UndoActionCount + i].Execute();

                UndoActionCount += actionCount;
            }
            finally
            {
                OnEnd();
            }
        }

        public void RedoTo(IUndoableAction action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (RedoActionCount < 1)
                throw new InvalidOperationException();

            // Find the action first to prevent endless loops and to only update UndoActions once
            // Do the loop from the end of the undo actions to skip searching any undo actions
            var i = UndoActionCount;
            for (; i < ActionStack.Count; i++)
            {
                if (ActionStack[i] == action)
                    break;
            }

            if (i >= ActionStack.Count)
                throw new InvalidOperationException();

            Redo(1 + i - UndoActionCount);
        }

        private void RemoveAction(int index)
        {
            var action = ActionStack[index];
            ActionStack.RemoveAt(index);
            (action as IDisposable)?.Dispose();
        }

        private void OnBegin()
        {
            BatchBegin?.Invoke(this, EventArgs.Empty);
        }

        private void OnEnd()
        {
            BatchEnd?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            for (var i = ActionStack.Count - 1; i >= 0; i--)
                (ActionStack[i] as IDisposable)?.Dispose();
        }
    }
}
