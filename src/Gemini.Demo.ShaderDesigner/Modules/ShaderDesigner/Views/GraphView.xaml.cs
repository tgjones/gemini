using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner.ViewModels;
using Gemini.Modules.GraphEditor.Controls;
using Gemini.Modules.Toolbox;
using Gemini.Modules.Toolbox.Models;

namespace Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner.Views
{
    /// <summary>
    /// Interaction logic for GraphView.xaml
    /// </summary>
    public partial class GraphView : UserControl
    {
        private Point _originalContentMouseDownPoint;

        private GraphViewModel ViewModel
        {
            get { return (GraphViewModel) DataContext; }
        }

        public GraphView()
        {
            InitializeComponent();
        }

        private void OnGraphControlRightMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            _originalContentMouseDownPoint = e.GetPosition(GraphControl);
            GraphControl.CaptureMouse();
            Mouse.OverrideCursor = Cursors.ScrollAll;
            e.Handled = true;
        }

        private void OnGraphControlRightMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = null;
            GraphControl.ReleaseMouseCapture();
            e.Handled = true;
        }

        private void OnGraphControlMouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed && GraphControl.IsMouseCaptured)
            {
                Point currentContentMousePoint = e.GetPosition(GraphControl);
                Vector dragOffset = currentContentMousePoint - _originalContentMouseDownPoint;

                ZoomAndPanControl.ContentOffsetX -= dragOffset.X;
                ZoomAndPanControl.ContentOffsetY -= dragOffset.Y;

                e.Handled = true;
            }
        }

        private void OnGraphControlMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ZoomAndPanControl.ZoomAboutPoint(
                ZoomAndPanControl.ContentScale + e.Delta / 1000.0f,
                e.GetPosition(GraphControl));

            e.Handled = true;
        }

        private void OnGraphControlConnectionDragStarted(object sender, ConnectionDragStartedEventArgs e)
        {
            var sourceConnector = (ConnectorViewModel) e.SourceConnector.DataContext;
            var currentDragPoint = Mouse.GetPosition(GraphControl);
            var connection = ViewModel.OnConnectionDragStarted(sourceConnector, currentDragPoint);
            e.Connection = connection;
        }

        private void OnGraphControlConnectionDragging(object sender, ConnectionDraggingEventArgs e)
        {
            var currentDragPoint = Mouse.GetPosition(GraphControl);
            var connection = (ConnectionViewModel) e.Connection;
            ViewModel.OnConnectionDragging(currentDragPoint, connection);
        }

        private void OnGraphControlConnectionDragCompleted(object sender, ConnectionDragCompletedEventArgs e)
        {
            var sourceConnector = (ConnectorViewModel) e.SourceConnector.DataContext;
            var destinationConnector = (e.DestinationConnectorItem != null)
                ? (ConnectorViewModel) e.DestinationConnectorItem.DataContext
                : null;
            var newConnection = (ConnectionViewModel) e.Connection;
            ViewModel.OnConnectionDragCompleted(newConnection, sourceConnector, destinationConnector);
        }

        private void OnGraphControlDragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(ToolboxDragDrop.DataFormat))
                e.Effects = DragDropEffects.None;
        }

        private void OnGraphControlDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(ToolboxDragDrop.DataFormat))
            {
                var mousePosition = e.GetPosition(GraphControl);

                var toolboxItem = (ToolboxItem) e.Data.GetData(ToolboxDragDrop.DataFormat);
                var element = (ElementViewModel) Activator.CreateInstance(toolboxItem.ItemType);
                element.X = mousePosition.X;
                element.Y = mousePosition.Y;

                var viewModel = (GraphViewModel) DataContext;
                viewModel.Elements.Add(element);
            }
        }
    }
}
