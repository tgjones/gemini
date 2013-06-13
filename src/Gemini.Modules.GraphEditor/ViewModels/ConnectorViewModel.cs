using System;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;

namespace Gemini.Modules.GraphEditor.ViewModels
{
    public class ConnectorViewModel : PropertyChangedBase
    {
        public event EventHandler PositionChanged;

        private readonly ElementViewModel _element;
        public ElementViewModel Element
        {
            get { return _element; }
        }

        private readonly BindableCollection<ConnectionViewModel> _connections;
        public IObservableCollection<ConnectionViewModel> Connections
        {
            get { return _connections; }
        }

        private Color _color = Colors.Black;
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                NotifyOfPropertyChange(() => Color);
            }
        }

        private Point _position;
        public Point Position
        {
            get { return _position; }
            set
            {
                _position = value;
                NotifyOfPropertyChange(() => Position);
                RaisePositionChanged();
            }
        }

        public ConnectorViewModel(ElementViewModel element)
        {
            _element = element;
            _connections = new BindableCollection<ConnectionViewModel>();
        }

        private void RaisePositionChanged()
        {
            var handler = PositionChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}