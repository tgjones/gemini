using System.Windows;
using Caliburn.Micro;

namespace Gemini.Modules.StatusBar.ViewModels
{
    public class StatusBarItemViewModel : PropertyChangedBase
    {
        private int _index;
        public int Index
        {
            get { return _index; }
            internal set
            {
                _index = value;
                NotifyOfPropertyChange(() => Index);
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        private readonly GridLength _width;
        public GridLength Width
        {
            get { return _width; }
        }

        public StatusBarItemViewModel(string message, GridLength width)
        {
            _message = message;
            _width = width;
        }
    }
}