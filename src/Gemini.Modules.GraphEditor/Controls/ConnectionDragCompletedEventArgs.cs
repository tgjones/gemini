using System.Windows;

namespace Gemini.Modules.GraphEditor.Controls
{
    public class ConnectionDragCompletedEventArgs : ConnectionDragEventArgs
    {
        private readonly object _connection;
        private readonly object _destinationConnector;

        public object Connection
        {
            get { return _connection; }
        }

        public object DestinationConnector
        {
            get { return _destinationConnector; }
        }

        internal ConnectionDragCompletedEventArgs(RoutedEvent routedEvent, object source, object node, object connection, object sourceConnector, object destinationConnector)
            : base(routedEvent, source, node, sourceConnector)
        {
            _connection = connection;
            _destinationConnector = destinationConnector;
        }
    }

    public delegate void ConnectionDragCompletedEventHandler(object sender, ConnectionDragCompletedEventArgs e);
}