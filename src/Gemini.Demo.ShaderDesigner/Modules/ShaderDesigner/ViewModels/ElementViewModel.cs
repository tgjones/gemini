using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;

namespace Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner.ViewModels
{
    public abstract class ElementViewModel : PropertyChangedBase
    {
        public event EventHandler InputConnectorConnectionChanged;

        public const int PreviewSize = 100;

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

        public abstract BitmapSource PreviewImage { get; }

        private readonly BindableCollection<InputConnectorViewModel> _inputConnectors;
        public IList<InputConnectorViewModel> InputConnectors
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

        public IEnumerable<ConnectionViewModel> AttachedConnections
        {
            get
            {
                return _inputConnectors.Select(x => x.Connection)
                    .Union(_outputConnector.Connections)
                    .Where(x => x != null);
            }
        }

        protected ElementViewModel()
        {
            _inputConnectors = new BindableCollection<InputConnectorViewModel>();
            _name = GetType().Name;
        }

        protected void AddInputConnector(string name, Color color)
        {
            var inputConnector = new InputConnectorViewModel(this, name, color);
            inputConnector.ConnectionChanged += (sender, e) => RaiseInputConnectorConnectionChanged();
            _inputConnectors.Add(inputConnector);
        }

        protected void SetOutputConnector(string name, Color color, Func<BitmapSource> valueCallback)
        {
            OutputConnector = new OutputConnectorViewModel(this, name, color, valueCallback);
        }

        protected virtual void RaiseInputConnectorConnectionChanged()
        {
            EventHandler handler = InputConnectorConnectionChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}