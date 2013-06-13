using System.Windows;

namespace Gemini.Modules.GraphEditor.Controls
{
    public abstract class ConnectionDragEventArgs : RoutedEventArgs
    {
        private readonly ElementItem _elementItem;
        private readonly ConnectorItem _sourceConnectorItem;

        public ElementItem ElementItem
        {
            get { return _elementItem; }
        }

        public ConnectorItem SourceConnector
        {
            get { return _sourceConnectorItem; }
        }

        protected ConnectionDragEventArgs(RoutedEvent routedEvent, object source,
            ElementItem elementItem, ConnectorItem sourceConnectorItem)
            : base(routedEvent, source)
        {
            _elementItem = elementItem;
            _sourceConnectorItem = sourceConnectorItem;
        }
    }
}