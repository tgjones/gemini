using System.Windows;

namespace Gemini.Modules.GraphEditor.Controls
{
    public class ConnectionDragCompletedEventArgs : ConnectionDragEventArgs
    {
        private readonly object _connection;

        public object Connection
        {
            get { return _connection; }
        }

        internal ConnectionDragCompletedEventArgs(RoutedEvent routedEvent, object source, 
            ElementItem elementItem, object connection, ConnectorItem sourceConnectorItem)
            : base(routedEvent, source, elementItem, sourceConnectorItem)
        {
            _connection = connection;
        }
    }

    public delegate void ConnectionDragCompletedEventHandler(object sender, ConnectionDragCompletedEventArgs e);
}