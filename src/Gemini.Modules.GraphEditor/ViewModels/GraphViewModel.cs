using System.Linq;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Gemini.Framework;

namespace Gemini.Modules.GraphEditor.ViewModels
{
    public class GraphViewModel : Document
    {
        public override string DisplayName
        {
            get { return "[New Graph]"; }
        }

        private readonly BindableCollection<ElementViewModel> _elements;
        public IObservableCollection<ElementViewModel> Elements
        {
            get { return _elements; }
        }

        private readonly BindableCollection<ConnectionViewModel> _connections;
        public IObservableCollection<ConnectionViewModel> Connections
        {
            get { return _connections; }
        }

        public GraphViewModel()
        {
            _elements = new BindableCollection<ElementViewModel>();
            _connections = new BindableCollection<ConnectionViewModel>();

            var element1 = AddElement(100, 100, "Add");
            var element2 = AddElement(400, 250, "Multiply");

            Connections.Add(new ConnectionViewModel(
                element1.OutputConnector, 
                element2.InputConnectors[0]));

            element1.IsSelected = true;
        }

        public ElementViewModel AddElement(double x, double y, string name)
        {
            var element = new ElementViewModel { X = x, Y = y, Name = name };
            element.InputConnectors.Add(new InputConnectorViewModel(element, "Foreground", Colors.DodgerBlue));
            element.InputConnectors.Add(new InputConnectorViewModel(element, "Background", Colors.DarkSeaGreen));
            element.OutputConnector = new OutputConnectorViewModel(element, "Output", Colors.DodgerBlue);
            _elements.Add(element);
            return element;
        }

        public ConnectionViewModel OnConnectionDragStarted(ConnectorViewModel sourceConnector, Point currentDragPoint)
        {
            // TODO: Check that source connector is an output connector.

            var connection = new ConnectionViewModel
            {
                From = sourceConnector,
                ToPosition = currentDragPoint
            };

            Connections.Add(connection);

            return connection;
        }

        public void OnConnectionDragging(Point currentDragPoint, ConnectionViewModel connection)
        {
            connection.ToPosition = currentDragPoint;
        }

        public void OnConnectionDragCompleted(ConnectionViewModel newConnection, ConnectorViewModel sourceConnector, ConnectorViewModel destinationConnector)
        {
            if (destinationConnector == null)
            {
                Connections.Remove(newConnection);
                return;
            }

            // TODO: Check that destination connector is an input connector.

            if (sourceConnector.Element == destinationConnector.Element)
            {
                Connections.Remove(newConnection);
                return;
            }

            var existingConnection = FindConnection(sourceConnector, destinationConnector);
            if (existingConnection != null)
                Connections.Remove(existingConnection);

            newConnection.To = destinationConnector;
        }

        private static ConnectionViewModel FindConnection(
            ConnectorViewModel sourceConnector, 
            ConnectorViewModel destinationConnector)
        {
            return sourceConnector.Connections.FirstOrDefault(connection => connection.To == destinationConnector);
        }
    }
}