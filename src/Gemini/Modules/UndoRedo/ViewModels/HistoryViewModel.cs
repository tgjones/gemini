using System.Collections.Specialized;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Gemini.Modules.UndoRedo.ViewModels
{
    [Export(typeof(IHistoryTool))]
    public class HistoryViewModel : Tool, IHistoryTool
    {
        private readonly BindableCollection<HistoryItemViewModel> _historyItems;
 
        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Right; }
        }

        public override string DisplayName
        {
            get { return "History"; }
        }

        private IUndoRedoManager _undoRedoManager;
        public IUndoRedoManager UndoRedoManager
        {
            get { return _undoRedoManager; }
            set
            {
                if (_undoRedoManager != null)
                {
                    _undoRedoManager.UndoStack.CollectionChanged -= OnUndoRedoStackChanged;
                    _undoRedoManager.RedoStack.CollectionChanged -= OnUndoRedoStackChanged;
                }

                _undoRedoManager = value;
                _undoRedoManager.UndoStack.CollectionChanged += OnUndoRedoStackChanged;
                _undoRedoManager.RedoStack.CollectionChanged += OnUndoRedoStackChanged;
                RefreshHistory();
            }
        }

        public IObservableCollection<HistoryItemViewModel> HistoryItems
        {
            get { return _historyItems; }
        }

        [ImportingConstructor]
        public HistoryViewModel(IShell shell)
        {
            _historyItems = new BindableCollection<HistoryItemViewModel>();

            shell.ActiveDocumentChanged += (sender, e) =>
            {
                UndoRedoManager = shell.ActiveItem.UndoRedoManager;
            };
            if (shell.ActiveItem != null)
                UndoRedoManager = shell.ActiveItem.UndoRedoManager;
        }

        private void OnUndoRedoStackChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshHistory();
        }

        private void RefreshHistory()
        {
            _historyItems.Clear();
            foreach (var action in _undoRedoManager.UndoStack)
                _historyItems.Add(new HistoryItemViewModel(action, HistoryItemType.Undo));
            foreach (var action in _undoRedoManager.RedoStack)
                _historyItems.Add(new HistoryItemViewModel(action, HistoryItemType.Redo));
        }
    }
}