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
                if (_undoRedoManager != null)
                {
                    _undoRedoManager.UndoStack.CollectionChanged += OnUndoRedoStackChanged;
                    _undoRedoManager.RedoStack.CollectionChanged += OnUndoRedoStackChanged;
                }
                RefreshHistory();
            }
        }

        private bool _internallyTriggeredChange;
        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                NotifyOfPropertyChange(() => SelectedIndex);
                TriggerInternalHistoryChange(() => UndoOrRedoToInternal(HistoryItems[value - 1]));
            }
        }

        public IObservableCollection<HistoryItemViewModel> HistoryItems
        {
            get { return _historyItems; }
        }

        [ImportingConstructor]
        public HistoryViewModel(IShell shell)
        {
            DisplayName = "History";

            _historyItems = new BindableCollection<HistoryItemViewModel>();

            if (shell == null)
                return;

            shell.ActiveDocumentChanged += (sender, e) =>
            {
                UndoRedoManager = (shell.ActiveItem != null) ? shell.ActiveItem.UndoRedoManager : null;
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
            if (_undoRedoManager != null)
            {
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

            if (!_internallyTriggeredChange)
                UpdateSelectedIndexOnly(_historyItems.Count);
        }

        public void UndoOrRedoTo(HistoryItemViewModel item)
        {
            TriggerInternalHistoryChange(() => UndoOrRedoToInternal(item));
            UpdateSelectedIndexOnly(_historyItems.IndexOf(_historyItems.First(x => x.Action == item.Action)) + 1);
        }

        private void TriggerInternalHistoryChange(System.Action callback)
        {
            _internallyTriggeredChange = true;
            callback();
            _internallyTriggeredChange = false;
        }

        private void UpdateSelectedIndexOnly(int selectedIndex)
        {
            _selectedIndex = selectedIndex;
            NotifyOfPropertyChange(() => SelectedIndex);
        }

        private void UndoOrRedoToInternal(HistoryItemViewModel item)
        {
            switch (item.ItemType)
            {
                case HistoryItemType.InitialState:
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