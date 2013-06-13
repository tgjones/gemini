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
            object node, object connection, object connector)
            : base(routedEvent, source, node, connector)
        {
            _connection = connection;
        }
    }

    public delegate void ConnectionDraggingEventHandler(object sender, ConnectionDraggingEventArgs e);
}