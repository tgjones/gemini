using System.Windows;

namespace Gemini.Modules.GraphEditor.Controls
{
    public class ConnectionDraggingEventArgs : ConnectionDragEventArgs
    {
        private readonly object _connection;

        public object Connection
        {
            get { return _connection; }
        }

        internal ConnectionDraggingEventArgs(RoutedEvent routedEvent, object source,
            ElementItem elementItem, object connection, ConnectorItem connectorItem)
            : base(routedEvent, source, elementItem, connectorItem)
        {
            _connection = connection;
        }
    }

    public delegate void ConnectionDraggingEventHandler(object sender, ConnectionDraggingEventArgs e);
}