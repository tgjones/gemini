using System;
using System.Windows;
using Caliburn.Micro;
using System.Windows.Media;
using System.ComponentModel;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels
{
    public abstract class ConnectionViewModel : PropertyChangedBase, IsSelectable
    {
        public abstract Type ConnectionType { get; set; }

        internal abstract OutputConnectorViewModel _from { get; set; }
        public OutputConnectorViewModel From
        {
            get { return _from; }
            private set
            {
                if (value.Type != ConnectionType) throw new Exception();
                if (_from != null)
                {
                    _from.PositionChanged -= OnFromPositionChanged;
                    _from.Connections.Remove(this);
                }

                _from = value;

                if (_from != null)
                {
                    _from.PositionChanged += OnFromPositionChanged;
                    _from.Connections.Add(this);
                    FromPosition = value.Position;
                }

                NotifyOfPropertyChange(() => From);
            }
        }

        [Browsable(false)]
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


        internal abstract InputConnectorViewModel _to { get; set; }
        public InputConnectorViewModel To
        {
            get { return _to; }
            set
            {
                if (value.Type != ConnectionType) throw new Exception();
                if (_to != null)
                {
                    _to.PositionChanged -= OnToPositionChanged;
                    _to.Connections.Remove(this);
                }

                _to = value;

                if (_to != null)
                {
                    _to.PositionChanged += OnToPositionChanged;
                    _to.Connections.Add(this);
                    ToPosition = _to.Position;
                }

                NotifyOfPropertyChange(() => To);
            }
        }

        private Point _fromPosition;
        public Point FromPosition
        {
            get { return _fromPosition; }
            set
            {
                _fromPosition = value;
                NotifyOfPropertyChange(() => FromPosition);
            }
        }

        internal abstract Brush _color { get; set; }
        public Brush Color
        {
            get { return _color; }
            set
            {
                _color = value;
                NotifyOfPropertyChange(() => Color);
            }
        }


        private Point _toPosition;
        public Point ToPosition
        {
            get { return _toPosition; }
            set
            {
                _toPosition = value;
                NotifyOfPropertyChange(() => ToPosition);
            }
        }

        public ConnectionViewModel(OutputConnectorViewModel from, InputConnectorViewModel to)
        {
            From = from;
            To = to;
        }

        public ConnectionViewModel(OutputConnectorViewModel from)
        {
            From = from;
        }

        private void OnFromPositionChanged(object sender, EventArgs e)
        {
            FromPosition = From.Position;
        }

        private void OnToPositionChanged(object sender, EventArgs e)
        {
            ToPosition = To.Position;
        }
    }
}