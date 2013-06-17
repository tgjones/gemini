using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner.Design;
using Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner.Util;
using Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner.ViewModels.Elements;
using Gemini.Framework;

namespace Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner.ViewModels
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

            var element1 = AddElement<ImageSource>(100, 50);
            element1.Bitmap = BitmapUtility.CreateFromBytes(DesignTimeImages.Desert);

            var element2 = AddElement<ImageSource>(100, 300);
            element2.Bitmap = BitmapUtility.CreateFromBytes(DesignTimeImages.Tulips);

            var element3 = AddElement<Add>(400, 250);

            Connections.Add(new ConnectionViewModel(
                element1.OutputConnector,
                element3.InputConnectors[0]));

            Connections.Add(new ConnectionViewModel(
                element2.OutputConnector,
                element3.InputConnectors[1]));

            element3.Process();

            element1.IsSelected = true;
        }

        public TElement AddElement<TElement>(double x, double y)
            where TElement : ElementViewModel, new()
        {
            var element = new TElement { X = x, Y = y };
            _elements.Add(element);
            return element;
        }

        public ConnectionViewModel OnConnectionDragStarted(ConnectorViewModel sourceConnector, Point currentDragPoint)
        {
            if (!(sourceConnector is OutputConnectorViewModel))
                return null;

            var connection = new ConnectionViewModel
            {
                From = (OutputConnectorViewModel) sourceConnector,
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
            if (destinationConnector == null 
                || !(destinationConnector is InputConnectorViewModel)
                || sourceConnector.Element == destinationConnector.Element)
            {
                Connections.Remove(newConnection);
                return;
            }

            var existingConnection = FindConnection(
                (OutputConnectorViewModel) sourceConnector, 
                (InputConnectorViewModel) destinationConnector);
            if (existingConnection != null)
                Connections.Remove(existingConnection);

            newConnection.To = (InputConnectorViewModel) destinationConnector;
        }

        private static ConnectionViewModel FindConnection(
            OutputConnectorViewModel sourceConnector, 
            InputConnectorViewModel destinationConnector)
        {
            return sourceConnector.Connections.FirstOrDefault(connection => connection.To == destinationConnector);
        }
    }
}