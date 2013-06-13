using System.Windows;

namespace Gemini.Modules.GraphEditor.Controls
{
    internal class ConnectorItemDragCompletedEventArgs : RoutedEventArgs
    {
        public ConnectorItemDragCompletedEventArgs(RoutedEvent routedEvent, object source) :
            base(routedEvent, source)
        {
        }
    }

    internal delegate void ConnectorItemDragCompletedEventHandler(object sender, ConnectorItemDragCompletedEventArgs e);
}