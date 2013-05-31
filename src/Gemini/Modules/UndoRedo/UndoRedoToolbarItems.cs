using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework.Results;
using Gemini.Framework.ToolBars;

namespace Gemini.Modules.UndoRedo
{
    public static class UndoRedoToolBarItems
    {
        public static ToolBarItemBase CreateUndoToolbarItem()
        {
            var undoRedoManager = IoC.Get<IUndoRedoManager>();
            var result = new ToolBarItem("Undo", ExecuteUndo, CanExecuteUndo)
                .WithGlobalShortcut(ModifierKeys.Control, Key.Z)
                .WithIcon();
            undoRedoManager.UndoStack.CollectionChanged += (sender, e) =>
                result.NotifyOfPropertyChange(() => result.CanExecute);
            return result;
        }

        private static IEnumerable<IResult> ExecuteUndo()
        {
            yield return new LambdaResult(c =>
            {
                var undoRedoManager = IoC.Get<IUndoRedoManager>();
                undoRedoManager.Undo(1);
            });
        }

        private static bool CanExecuteUndo()
        {
            return IoC.Get<IUndoRedoManager>().UndoStack.Any();
        }

        public static ToolBarItemBase CreateRedoToolbarItem()
        {
            var undoRedoManager = IoC.Get<IUndoRedoManager>();
            var result = new ToolBarItem("Redo", ExecuteRedo, CanExecuteRedo)
                .WithGlobalShortcut(ModifierKeys.Control, Key.Y)
                .WithIcon();
            undoRedoManager.RedoStack.CollectionChanged += (sender, e) =>
                result.NotifyOfPropertyChange(() => result.CanExecute);
            return result;
        }

        private static IEnumerable<IResult> ExecuteRedo()
        {
            yield return new LambdaResult(c =>
            {
                var undoRedoManager = IoC.Get<IUndoRedoManager>();
                undoRedoManager.Redo(1);
            });
        }

        private static bool CanExecuteRedo()
        {
            return IoC.Get<IUndoRedoManager>().RedoStack.Any();
        }
    }
}