using Caliburn.Micro;

namespace Gemini.Modules.GraphEditor.ViewModels
{
    public class ElementViewModel : PropertyChangedBase
    {
        private double _x;
        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
                NotifyOfPropertyChange(() => X);
            }
        }

        private double _y;
        public double Y
        {
            get { return _y; }
            set
            {
                _y = value;
                NotifyOfPropertyChange(() => Y);
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        private readonly BindableCollection<ConnectorViewModel> _inputConnectors;
        public IObservableCollection<ConnectorViewModel> InputConnectors
        {
            get { return _inputConnectors; }
        }

        private readonly BindableCollection<ConnectorViewModel> _outputConnectors;
        public IObservableCollection<ConnectorViewModel> OutputConnectors
        {
            get { return _outputConnectors; }
        }

        public ElementViewModel()
        {
            _inputConnectors = new BindableCollection<ConnectorViewModel>();
            _outputConnectors = new BindableCollection<ConnectorViewModel>();
        }
    }
}