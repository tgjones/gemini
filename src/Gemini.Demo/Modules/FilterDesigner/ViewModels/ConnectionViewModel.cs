using System;
using System.ComponentModel;
using System.Windows;
using Caliburn.Micro;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels
{
    public class ConnectionViewModel : PropertyChangedBase
    {
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

        private OutputConnectorViewModel _from;
        [Browsable(false)]
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
        [Browsable(false)]
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
        [Browsable(false)]
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
        [Browsable(false)]
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
