using Caliburn.Micro;

namespace Gemini.Modules.UndoRedo.Services
{
    public class UndoRedoManager : IUndoRedoManager
    {
        private readonly BindableCollection<IUndoableAction> _undoStack;
        private readonly BindableCollection<IUndoableAction> _redoStack;

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
            _undoStack = new BindableCollection<IUndoableAction>();
            _redoStack = new BindableCollection<IUndoableAction>();
        }

        public void ExecuteAction(IUndoableAction action)
        {
            action.Execute();
            Push(_undoStack, action);
            _redoStack.Clear();
        }

        public void Undo(int actionCount)
        {
            for (int i = 0; i < actionCount; i++)
            {
                var action = Pop(_undoStack);
                action.Undo();
                Push(_redoStack, action);
            }
        }

        public void Redo(int actionCount)
        {
            for (int i = 0; i < actionCount; i++)
            {
                var action = Pop(_redoStack);
                action.Execute();
                Push(_undoStack, action);
            }
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
    }
}