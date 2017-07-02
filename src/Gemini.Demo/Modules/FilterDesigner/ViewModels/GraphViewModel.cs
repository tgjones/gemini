using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Gemini.Demo.Modules.FilterDesigner.Design;
using Gemini.Demo.Modules.FilterDesigner.Util;
using Gemini.Demo.Modules.FilterDesigner.ViewModels.Elements;
using Gemini.Framework;
using Gemini.Modules.Inspector;
using ImageSource = Gemini.Demo.Modules.FilterDesigner.ViewModels.Elements.ImageSource;
using System.Windows.Media.Imaging;
using Gemini.Demo.Modules.FilterDesigner.ViewModels.Connection;
using System;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels
{
    [Export(typeof(GraphViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GraphViewModel : Document
    {
        private readonly IInspectorTool _inspectorTool;

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

        public IEnumerable<IsSelectable> SelectedElements
        {
            get { return _elements.Where<IsSelectable>(x => x.IsSelected).Concat(_connections.Where(x=>x.IsSelected)); }
        }
          
        [ImportingConstructor]
        public GraphViewModel(IInspectorTool inspectorTool)
        {
            DisplayName = "[New Graph]";

            _elements = new BindableCollection<ElementViewModel>();
            _connections = new BindableCollection<ConnectionViewModel>();

            _inspectorTool = inspectorTool;

            ImageSource element1 = AddElement<ImageSource>(100, 50);
            element1.Bitmap = BitmapUtility.CreateFromBytes(DesignTimeImages.Desert);

            var element2 = AddElement<ColorInput>(100, 300);
            element2.Color = Colors.Green;

            var element3 = AddElement<Multiply>(400, 250);

            Connections.Add(element1.OutputConnectors[0].Connect(element3.InputConnectors[0]));
            Connections.Add(element2.OutputConnectors[0].Connect(element3.InputConnectors[1]));

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
            var tt = sourceConnector.Type;

            var connection = sourceConnector.GetNewConnection();
            connection.ToPosition = currentDragPoint;
     

            Connections.Add(connection);

            return connection;
        }

        public void OnConnectionDragging(Point currentDragPoint, ConnectionViewModel connection)
        {
            // If current drag point is close to an input connector, show its snapped position.
            var nearbyConnector = FindNearbyInputConnector(currentDragPoint, connection.ConnectionType);
            connection.ToPosition = (nearbyConnector != null)
                ? nearbyConnector.Position
                : currentDragPoint;
        }

        public void OnConnectionDragCompleted(Point currentDragPoint, ConnectionViewModel newConnection, ConnectorViewModel sourceConnector)
        {
            var nearbyConnector = FindNearbyInputConnector(currentDragPoint, newConnection.ConnectionType);

            if (nearbyConnector == null || sourceConnector.Element == nearbyConnector.Element)
            {
                Connections.Remove(newConnection);
                return;
            }

            newConnection.To = nearbyConnector;
        }

        private InputConnectorViewModel FindNearbyInputConnector(Point mousePosition, Type t)
        {
            return Elements.SelectMany(x => x.InputConnectors)
                .FirstOrDefault(x => x.Type == t && AreClose(x.Position, mousePosition, 10));
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

        public void OnSelectionChanged()
        {
            var selectedElements = SelectedElements.ToList();

            if (selectedElements.Count == 1)
                _inspectorTool.SelectedObject = new InspectableObjectBuilder()
                    .WithObjectProperties(selectedElements[0], x => true)
                    .ToInspectableObject();
            else
                _inspectorTool.SelectedObject = null;
        }
    }
}