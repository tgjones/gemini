using Caliburn.Micro;

namespace Gemini.Modules.ErrorList
{
    public class ErrorListItem : PropertyChangedBase
    {
        private ErrorListItemType _itemType;
        public ErrorListItemType ItemType
        {
            get { return _itemType; }
            set
            {
                _itemType = value;
                NotifyOfPropertyChange(() => ItemType);
            }
        }

        private int _number;
        public int Number
        {
            get { return _number; }
            set
            {
                _number = value;
                NotifyOfPropertyChange(() => Number);
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                NotifyOfPropertyChange(() => Description);
            }
        }

        private string _file;
        public string File
        {
            get { return _file; }
            set
            {
                _file = value;
                NotifyOfPropertyChange(() => File);
            }
        }

        private int? _line;
        public int? Line
        {
            get { return _line; }
            set
            {
                _line = value;
                NotifyOfPropertyChange(() => Line);
            }
        }

        private int? _column;
        public int? Column
        {
            get { return _column; }
            set
            {
                _column = value;
                NotifyOfPropertyChange(() => Column);
            }
        }
    }
}