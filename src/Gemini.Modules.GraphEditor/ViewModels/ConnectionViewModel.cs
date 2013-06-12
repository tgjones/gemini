using Caliburn.Micro;

namespace Gemini.Modules.GraphEditor.ViewModels
{
    public class ConnectionViewModel : PropertyChangedBase
    {
        private readonly ConnectorViewModel _from;
        public ConnectorViewModel From
        {
            get { return _from; }
        }

        private readonly ConnectorViewModel _to;
        public ConnectorViewModel To
        {
            get { return _to; }
        }

        public ConnectionViewModel(ConnectorViewModel from, ConnectorViewModel to)
        {
            _from = from;
            _from.Connections.Add(this);

            _to = to;
            _to.Connections.Add(this);
        }
    }
}