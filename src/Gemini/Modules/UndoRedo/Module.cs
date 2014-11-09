using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Modules.UndoRedo.Commands;

namespace Gemini.Modules.UndoRedo
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase//, IPartImportsSatisfiedNotification
    {
        //[Import]
        //private IShell _shell;

        //void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        //{
        //    CommandService.AddCommand(new GeminiCommand(UndoRedoCommands.Undo,
        //        async p => _shell.ActiveItem.UndoRedoManager.Undo(1),
        //        (sender, e) =>
        //        {
        //            ((GeminiCommand) sender).Enabled = (_shell.ActiveItem != null &&
        //                                                _shell.ActiveItem.UndoRedoManager.UndoStack.Any());
        //        }));
        //    CommandService.AddCommand(new GeminiCommand(UndoRedoCommands.Redo,
        //        async p => _shell.ActiveItem.UndoRedoManager.Redo(1),
        //        (sender, e) =>
        //        {
        //            ((GeminiCommand) sender).Enabled = (_shell.ActiveItem != null &&
        //                                                _shell.ActiveItem.UndoRedoManager.RedoStack.Any());
        //        }));

        //    NotifyCollectionChangedEventHandler handler = (sender, e) => CommandManager.InvalidateRequerySuggested();

        //    _shell.ActiveDocumentChanging += (sender, e) =>
        //    {
        //        if (_shell.ActiveItem != null)
        //        {
        //            _shell.ActiveItem.UndoRedoManager.UndoStack.CollectionChanged -= handler;
        //            _shell.ActiveItem.UndoRedoManager.RedoStack.CollectionChanged -= handler;
        //        }
        //    };

        //    _shell.ActiveDocumentChanged += (sender, e) =>
        //    {
        //        if (_shell.ActiveItem != null)
        //        {
        //            _shell.ActiveItem.UndoRedoManager.UndoStack.CollectionChanged += handler;
        //            _shell.ActiveItem.UndoRedoManager.RedoStack.CollectionChanged += handler;
        //        }
        //        CommandManager.InvalidateRequerySuggested();
        //    };

        //    if (_shell.ActiveItem != null)
        //    {
        //        _shell.ActiveItem.UndoRedoManager.UndoStack.CollectionChanged += handler;
        //        _shell.ActiveItem.UndoRedoManager.RedoStack.CollectionChanged += handler;
        //    }
        //}
    }
}