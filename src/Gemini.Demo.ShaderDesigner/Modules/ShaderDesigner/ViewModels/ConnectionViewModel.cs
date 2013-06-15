using System;
using System.Windows;
using Caliburn.Micro;

namespace Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner.ViewModels
{
    public class ConnectionViewModel : PropertyChangedBase
    {
        private ConnectorViewModel _from;
        public ConnectorViewModel From
        {
            get { return _from; }
            set
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

        private ConnectorViewModel _to;
        public ConnectorViewModel To
        {
            get { return _to; }
            set
            {
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

        public ConnectionViewModel(ConnectorViewModel from, ConnectorViewModel to)
        {
            From = from;
            To = to;
        }

        public ConnectionViewModel()
        {
            
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