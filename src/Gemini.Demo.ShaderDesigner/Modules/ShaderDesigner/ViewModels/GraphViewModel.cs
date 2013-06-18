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
            // If current drag point is close to an input connector, show its snapped position.
            var nearbyConnector = FindNearbyInputConnector(currentDragPoint);
            connection.ToPosition = (nearbyConnector != null)
                ? nearbyConnector.Position
                : currentDragPoint;
        }

        public void OnConnectionDragCompleted(Point currentDragPoint, ConnectionViewModel newConnection, ConnectorViewModel sourceConnector)
        {
            var nearbyConnector = FindNearbyInputConnector(currentDragPoint);

            if (nearbyConnector == null || sourceConnector.Element == nearbyConnector.Element)
            {
                Connections.Remove(newConnection);
                return;
            }

            var existingConnection = nearbyConnector.Connection;
            if (existingConnection != null)
                Connections.Remove(existingConnection);

            newConnection.To = nearbyConnector;
        }

        private InputConnectorViewModel FindNearbyInputConnector(Point mousePosition)
        {
            return Elements.SelectMany(x => x.InputConnectors)
                .FirstOrDefault(x => AreClose(x.Position, mousePosition, 10));
        }

        private static bool AreClose(Point point1, Point point2, double distance)
        {
            return (point1 - point2).Length < distance;
        }

        public void DeleteElement(ElementViewModel element)
        {
            Connections.RemoveRange(element.AttachedConnections);
            Elements.Remove(element);
        }

        public void DeleteSelectedElements()
        {
            Elements.Where(x => x.IsSelected)
                .ToList()
                .ForEach(DeleteElement);
        }
    }
}