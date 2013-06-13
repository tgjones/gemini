using System.Windows;

namespace Gemini.Modules.GraphEditor.Controls
{
    public class ConnectionDragStartedEventArgs : ConnectionDragEventArgs
    {
        public object Connection { get; set; }

        public ConnectionDragStartedEventArgs(RoutedEvent routedEvent, object source,
            object element, object connector)
            : base(routedEvent, source, element, connector)
        {
        }
    }

    public delegate void ConnectionDragStartedEventHandler(object sender, ConnectionDragStartedEventArgs e);
}