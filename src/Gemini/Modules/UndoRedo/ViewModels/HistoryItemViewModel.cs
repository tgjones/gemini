using Caliburn.Micro;

namespace Gemini.Modules.UndoRedo.ViewModels
{
    public class HistoryItemViewModel : PropertyChangedBase
    {
        private readonly IUndoableAction _action;

        public IUndoableAction Action
        {
            get { return _action; }
        }

        private readonly string _name;
        public string Name
        {
            get { return _name ?? _action.Name; }
        }

        private readonly HistoryItemType _itemType;
        public HistoryItemType ItemType
        {
            get { return _itemType; }
        }

        public HistoryItemViewModel(IUndoableAction action, HistoryItemType itemType)
        {
            _action = action;
            _itemType = itemType;
        }

        public HistoryItemViewModel(string name, HistoryItemType itemType)
        {
            _name = name;
            _itemType = itemType;
        }
    }
}