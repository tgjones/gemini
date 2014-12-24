using System;
using System.Windows;
using Caliburn.Micro;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels
{
    public class ConnectionViewModel : PropertyChangedBase
    {
        private OutputConnectorViewModel _from;
        public OutputConnectorViewModel From
        {
            get { return _from; }
            private set
            {
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

        private InputConnectorViewModel _to;
        public InputConnectorViewModel To
        {
            get { return _to; }
            set
            {
                if (_to != null)
                {
                    _to.PositionChanged -= OnToPositionChanged;
                    _to.Connection = null;
                }

                _to = value;

                if (_to != null)
                {
                    _to.PositionChanged += OnToPositionChanged;
                    _to.Connection = this;
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