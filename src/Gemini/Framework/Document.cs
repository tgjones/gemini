using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Gemini.Framework.Commands;
using Gemini.Framework.Threading;
using Gemini.Modules.Shell.Commands;
using Gemini.Modules.UndoRedo;
using Gemini.Modules.UndoRedo.Commands;
using Gemini.Modules.UndoRedo.Services;
using Microsoft.Win32;

namespace Gemini.Framework
{
	public abstract class Document : LayoutItemBase, IDocument, 
        ICommandHandler<UndoCommandDefinition>,
        ICommandHandler<RedoCommandDefinition>,
        ICommandHandler<SaveFileCommandDefinition>
	{
	    private IUndoRedoManager _undoRedoManager;
	    public IUndoRedoManager UndoRedoManager
	    {
            get { return _undoRedoManager ?? (_undoRedoManager = new UndoRedoManager()); }
	    }

		private ICommand _closeCommand;
		public override ICommand CloseCommand
		{
		    get { return _closeCommand ?? (_closeCommand = new RelayCommand(p => TryClose(null), p => true)); }
		}

	    void ICommandHandler<UndoCommandDefinition>.Update(Command command)
	    {
            command.Enabled = UndoRedoManager.UndoStack.Any();
	    }

	    Task ICommandHandler<UndoCommandDefinition>.Run(Command command)
	    {
            UndoRedoManager.Undo(1);
            return TaskUtility.Completed;
	    }

        void ICommandHandler<RedoCommandDefinition>.Update(Command command)
        {
            command.Enabled = UndoRedoManager.RedoStack.Any();
        }

        Task ICommandHandler<RedoCommandDefinition>.Run(Command command)
        {
            UndoRedoManager.Redo(1);
            return TaskUtility.Completed;
        }

        void ICommandHandler<SaveFileCommandDefinition>.Update(Command command)
        {
            command.Enabled = this is IPersistedDocument;
        }

	    async Task ICommandHandler<SaveFileCommandDefinition>.Run(Command command)
	    {
	        var persistedDocument = this as IPersistedDocument;
	        if (persistedDocument == null)
	            return;

	        // If file has never been saved, show Save As dialog.
	        string filePath;
	        if (persistedDocument.IsNew)
	        {
	            var dialog = new SaveFileDialog();
	            dialog.FileName = persistedDocument.FileName;
	            if (dialog.ShowDialog() != true)
	                return;
	            filePath = dialog.FileName;
	        }
	        else
	        {
	            filePath = persistedDocument.FilePath;
	        }

	        // Save file.
	        await persistedDocument.Save(filePath);
	    }
	}
}