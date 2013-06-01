using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework.Results;
using Gemini.Framework.Services;
using Gemini.Framework.ToolBars;

namespace Gemini.Modules.UndoRedo
{
    public static class UndoRedoToolBarItems
    {
        public static ToolBarItemBase CreateUndoToolbarItem()
        {
            var result = new ToolBarItem("Undo", ExecuteUndo, CanExecuteUndo)
                .WithGlobalShortcut(ModifierKeys.Control, Key.Z)
                .WithIcon();

            NotifyCollectionChangedEventHandler handler = (sender, e) =>
                result.NotifyOfPropertyChange(() => result.CanExecute);

            var shell = IoC.Get<IShell>();
            shell.ActiveDocumentChanging += (sender, e) =>
            {
                if (shell.ActiveItem != null)
                    shell.ActiveItem.UndoRedoManager.UndoStack.CollectionChanged -= handler;
            };
            shell.ActiveDocumentChanged += (sender, e) =>
            {
                if (shell.ActiveItem != null)
                    shell.ActiveItem.UndoRedoManager.UndoStack.CollectionChanged += handler;
                result.NotifyOfPropertyChange(() => result.CanExecute);
            };

            if (shell.ActiveItem != null)
                shell.ActiveItem.UndoRedoManager.UndoStack.CollectionChanged += handler;

            return result;
        }

        private static IEnumerable<IResult> ExecuteUndo()
        {
            yield return new LambdaResult(c => IoC.Get<IShell>().ActiveItem.UndoRedoManager.Undo(1));
        }

        private static bool CanExecuteUndo()
        {
            return IoC.Get<IShell>().ActiveItem.UndoRedoManager.UndoStack.Any();
        }

        public static ToolBarItemBase CreateRedoToolbarItem()
        {
            var result = new ToolBarItem("Redo", ExecuteRedo, CanExecuteRedo)
                .WithGlobalShortcut(ModifierKeys.Control, Key.Y)
                .WithIcon();

            NotifyCollectionChangedEventHandler handler = (sender, e) =>
                result.NotifyOfPropertyChange(() => result.CanExecute);

            var shell = IoC.Get<IShell>();
            shell.ActiveDocumentChanging += (sender, e) =>
            {
                if (shell.ActiveItem != null)
                    shell.ActiveItem.UndoRedoManager.RedoStack.CollectionChanged -= handler;
            };
            shell.ActiveDocumentChanged += (sender, e) =>
            {
                if (shell.ActiveItem != null)
                    shell.ActiveItem.UndoRedoManager.RedoStack.CollectionChanged += handler;
                result.NotifyOfPropertyChange(() => result.CanExecute);
            };

            if (shell.ActiveItem != null)
                shell.ActiveItem.UndoRedoManager.RedoStack.CollectionChanged += handler;

            return result;
        }

        private static IEnumerable<IResult> ExecuteRedo()
        {
            yield return new LambdaResult(c => IoC.Get<IShell>().ActiveItem.UndoRedoManager.Redo(1));
        }

        private static bool CanExecuteRedo()
        {
            return IoC.Get<IShell>().ActiveItem.UndoRedoManager.RedoStack.Any();
        }
    }
}