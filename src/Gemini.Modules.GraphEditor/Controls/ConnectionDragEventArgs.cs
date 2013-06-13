using System.Windows;

namespace Gemini.Modules.GraphEditor.Controls
{
    public abstract class ConnectionDragEventArgs : RoutedEventArgs
    {
        private readonly object _element;
        private readonly object _sourceConnector;

        public object Element
        {
            get { return _element; }
        }

        public object SourceConnector
        {
            get { return _sourceConnector; }
        }

        protected ConnectionDragEventArgs(RoutedEvent routedEvent, object source,
            object element, object sourceConnector)
            : base(routedEvent, source)
        {
            _element = element;
            _sourceConnector = sourceConnector;
        }
    }
}