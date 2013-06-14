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

        private readonly BindableCollection<InputConnectorViewModel> _inputConnectors;
        public IObservableCollection<InputConnectorViewModel> InputConnectors
        {
            get { return _inputConnectors; }
        }

        private OutputConnectorViewModel _outputConnector;
        public OutputConnectorViewModel OutputConnector
        {
            get { return _outputConnector; }
            set
            {
                _outputConnector = value;
                NotifyOfPropertyChange(() => OutputConnector);
            }
        }

        public ElementViewModel()
        {
            _inputConnectors = new BindableCollection<InputConnectorViewModel>();
        }
    }
}