using System.Windows;

namespace Gemini.Modules.GraphEditor.Controls
{
    public class QueryConnectionFeedbackEventArgs : ConnectionDragEventArgs
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

        public bool ConnectionOk { get; set; }
        public object FeedbackIndicator { get; set; }

        internal QueryConnectionFeedbackEventArgs(RoutedEvent routedEvent, object source,
            object node, object connection, object sourceConnector, object destinationConnector)
            : base(routedEvent, source, node, sourceConnector)
        {
            _connection = connection;
            _destinationConnector = destinationConnector;
        }
    }

    public delegate void QueryConnectionFeedbackEventHandler(object sender, QueryConnectionFeedbackEventArgs e);
}