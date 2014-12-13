using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Gemini.Framework.Commands;
using Gemini.Framework.Threading;
using Gemini.Modules.UndoRedo;
using Gemini.Modules.UndoRedo.Commands;
using Gemini.Modules.UndoRedo.Services;

namespace Gemini.Framework
{
	public abstract class Document : LayoutItemBase, IDocument, 
        ICommandHandler<UndoCommandDefinition>,
        ICommandHandler<RedoCommandDefinition>
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
	}
}