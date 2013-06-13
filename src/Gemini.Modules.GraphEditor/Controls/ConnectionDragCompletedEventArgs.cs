using System.Windows;

namespace Gemini.Modules.GraphEditor.Controls
{
    public class ConnectionDragCompletedEventArgs : ConnectionDragEventArgs
    {
        private readonly object _connection;
        private readonly ConnectorItem _destinationConnectorItem;

        public object Connection
        {
            get { return _connection; }
        }

        public ConnectorItem DestinationConnectorItem
        {
            get { return _destinationConnectorItem; }
        }

        internal ConnectionDragCompletedEventArgs(RoutedEvent routedEvent, object source, 
            ElementItem elementItem, object connection, ConnectorItem sourceConnectorItem, ConnectorItem destinationConnectorItem)
            : base(routedEvent, source, elementItem, sourceConnectorItem)
        {
            _connection = connection;
            _destinationConnectorItem = destinationConnectorItem;
        }
    }

    public delegate void ConnectionDragCompletedEventHandler(object sender, ConnectionDragCompletedEventArgs e);
}