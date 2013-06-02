using System;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
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
            _historyItems.Add(new HistoryItemViewModel("Initial State",
                (_undoRedoManager.UndoStack.Any() ? HistoryItemType.InitialState : HistoryItemType.Current)));
            for (int i = 0; i < _undoRedoManager.UndoStack.Count; i++)
                _historyItems.Add(new HistoryItemViewModel(_undoRedoManager.UndoStack[i],
                    (i == _undoRedoManager.UndoStack.Count - 1) ? HistoryItemType.Current : HistoryItemType.Undo));
            for (int i = _undoRedoManager.RedoStack.Count - 1; i >= 0; i--)
                _historyItems.Add(new HistoryItemViewModel(
                    _undoRedoManager.RedoStack[i], 
                    HistoryItemType.Redo));
        }

        public void UndoOrRedoTo(HistoryItemViewModel item)
        {
            switch (item.ItemType)
            {
                case HistoryItemType.InitialState :
                    _undoRedoManager.UndoAll();
                    break;
                case HistoryItemType.Undo:
                    _undoRedoManager.UndoTo(item.Action);
                    break;
                case HistoryItemType.Current:
                    break;
                case HistoryItemType.Redo:
                    _undoRedoManager.RedoTo(item.Action);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}