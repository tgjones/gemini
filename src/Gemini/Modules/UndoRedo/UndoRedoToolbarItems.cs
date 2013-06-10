using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Framework.Services;
using Gemini.Modules.MainMenu.Models;
using Gemini.Modules.ToolBars.Models;

namespace Gemini.Modules.UndoRedo
{
    public class UndoMenuItem : MenuItem
    {
        public UndoMenuItem()
            : base("Undo", UndoRedoToolBarItems.ExecuteUndo, UndoRedoToolBarItems.CanExecuteUndo)
        {
            WithGlobalShortcut(ModifierKeys.Control, Key.Z).WithIcon();
            UndoRedoToolBarItems.SubscribeToHistoryEvents(this, m => m.UndoStack);
        }
    }

    public class RedoMenuItem : MenuItem
    {
        public RedoMenuItem()
            : base("Redo", UndoRedoToolBarItems.ExecuteRedo, UndoRedoToolBarItems.CanExecuteRedo)
        {
            WithGlobalShortcut(ModifierKeys.Control, Key.Y).WithIcon();
            UndoRedoToolBarItems.SubscribeToHistoryEvents(this, m => m.RedoStack);
        }
    }

    public class UndoToolBarItem : ToolBarItem
    {
        public UndoToolBarItem()
            : base("Undo", UndoRedoToolBarItems.ExecuteUndo, UndoRedoToolBarItems.CanExecuteUndo)
        {
            WithGlobalShortcut(ModifierKeys.Control, Key.Z).WithIcon();
            UndoRedoToolBarItems.SubscribeToHistoryEvents(this, m => m.UndoStack);
        }
    }

    public class RedoToolBarItem : ToolBarItem
    {
        public RedoToolBarItem()
            : base("Redo", UndoRedoToolBarItems.ExecuteRedo, UndoRedoToolBarItems.CanExecuteRedo)
        {
            WithGlobalShortcut(ModifierKeys.Control, Key.Y).WithIcon();
            UndoRedoToolBarItems.SubscribeToHistoryEvents(this, m => m.RedoStack);
        }
    }

    internal static class UndoRedoToolBarItems
    {
        public static IEnumerable<IResult> ExecuteUndo()
        {
            yield return new LambdaResult(c => IoC.Get<IShell>().ActiveItem.UndoRedoManager.Undo(1));
        }

        public static bool CanExecuteUndo()
        {
            return IoC.Get<IShell>().ActiveItem != null && IoC.Get<IShell>().ActiveItem.UndoRedoManager.UndoStack.Any();
        }

        public static IEnumerable<IResult> ExecuteRedo()
        {
            yield return new LambdaResult(c => IoC.Get<IShell>().ActiveItem.UndoRedoManager.Redo(1));
        }

        public static bool CanExecuteRedo()
        {
            return IoC.Get<IShell>().ActiveItem != null && IoC.Get<IShell>().ActiveItem.UndoRedoManager.RedoStack.Any();
        }

        public static void SubscribeToHistoryEvents(IExecutableItem toolBarItem,
            Func<IUndoRedoManager, IObservableCollection<IUndoableAction>> stackCallback)
        {
            NotifyCollectionChangedEventHandler handler = (sender, e) => toolBarItem.RaiseCanExecuteChanged();

            var shell = IoC.Get<IShell>();
            shell.ActiveDocumentChanging += (sender, e) =>
            {
                if (shell.ActiveItem != null)
                    stackCallback(shell.ActiveItem.UndoRedoManager).CollectionChanged -= handler;
            };
            shell.ActiveDocumentChanged += (sender, e) =>
            {
                if (shell.ActiveItem != null)
                    stackCallback(shell.ActiveItem.UndoRedoManager).CollectionChanged += handler;
                toolBarItem.RaiseCanExecuteChanged();
            };

            if (shell.ActiveItem != null)
                stackCallback(shell.ActiveItem.UndoRedoManager).CollectionChanged += handler;
        }
    }
}