using System.ComponentModel;
using System.Windows.Input;
using Gemini.Modules.UndoRedo;
using Gemini.Modules.UndoRedo.Services;

namespace Gemini.Framework
{
	public abstract class Document : LayoutItemBase, IDocument
	{
        [Browsable(false)]
        public override string DisplayName
        {
            get { return base.DisplayName; }
            set { base.DisplayName = value; }
        }

	    private IUndoRedoManager _undoRedoManager;
	    public IUndoRedoManager UndoRedoManager
	    {
            get { return _undoRedoManager ?? (_undoRedoManager = new UndoRedoManager()); }
	    }

		private ICommand _closeCommand;
		public ICommand CloseCommand
		{
			get
			{
				return _closeCommand ?? (_closeCommand = new RelayCommand(p => TryClose(null), p => true));
			}
		}
	}
}