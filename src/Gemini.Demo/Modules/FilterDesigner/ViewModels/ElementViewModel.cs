using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels
{
    public interface IsSelectable
    {
        bool IsSelected { get; set; }
    }

    public abstract class ElementViewModel : PropertyChangedBase, IsSelectable
    {
        public const double PreviewSize = 100;

        private double _x;

        [Browsable(false)]
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

        [Browsable(false)]
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

        [Browsable(false)]
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

        [Browsable(false)]
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

        private readonly BindableCollection<OutputConnectorViewModel> _outputConnectors;
        public IList<OutputConnectorViewModel> OutputConnectors
        {
            get { return _outputConnectors; }
        }

        public IEnumerable<ConnectionViewModel> AttachedConnections
        {
            get
            {
                return _inputConnectors.SelectMany(x => x.Connections).Union(_outputConnectors.SelectMany(x => x.Connections));
            }
        }

        protected ElementViewModel()
        {
            _inputConnectors = new BindableCollection<InputConnectorViewModel>();
            _outputConnectors = new BindableCollection<OutputConnectorViewModel>();

            _name = GetType().Name;
        }

        protected void AddInputConnector(InputConnectorViewModel inputConnector)
        {
            _inputConnectors.Add(inputConnector);
        }

        protected void AddOutputConnector(OutputConnectorViewModel outputConnector)
        {
            OutputConnectors.Add(outputConnector);
        }
    }
}