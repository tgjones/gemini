using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gemini.Framework;

namespace Gemini.Modules.GraphEditor.Controls
{
    public class ConnectorItem : ContentControl
    {
        private Point _lastMousePosition;
        private bool _isDragging;

        static ConnectorItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConnectorItem),
                new FrameworkPropertyMetadata(typeof(ConnectorItem)));
        }

        #region Dependency properties

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            "Position", typeof(Point), typeof(ConnectorItem));

        public Point Position
        {
            get { return (Point) GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        #endregion

        #region Routed events

        internal static readonly RoutedEvent ConnectorDragStartedEvent = EventManager.RegisterRoutedEvent(
            "ConnectorDragStarted", RoutingStrategy.Bubble, typeof(ConnectorItemDragStartedEventHandler), 
            typeof(ConnectorItem));

        internal static readonly RoutedEvent ConnectorDraggingEvent = EventManager.RegisterRoutedEvent(
            "ConnectorDragging", RoutingStrategy.Bubble, typeof(ConnectorItemDraggingEventHandler), 
            typeof(ConnectorItem));

        internal static readonly RoutedEvent ConnectorDragCompletedEvent = EventManager.RegisterRoutedEvent(
            "ConnectorDragCompleted", RoutingStrategy.Bubble, typeof(ConnectorItemDragCompletedEventHandler), 
            typeof(ConnectorItem));

        #endregion

        private GraphControl ParentGraphControl
        {
            get { return VisualTreeUtility.FindParent<GraphControl>(this); }
        }

        internal ElementItem ParentElementItem
        {
            get { return VisualTreeUtility.FindParent<ElementItem>(this); }
        }

        public ConnectorItem()
        {
            LayoutUpdated += OnLayoutUpdated;
        }

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            UpdatePosition();
        }

        /// <summary>
        /// Computes the coordinates, relative to the parent <see cref="GraphControl" />, of this connector.
        /// This is used to correctly position any connections that may be connected to this connector.
        /// (Say that 10 times fast.)
        /// </summary>
        private void UpdatePosition()
        {
            var parentGraphControl = VisualTreeUtility.FindParent<GraphControl>(this);
            if (parentGraphControl == null)
                return;

            var centerPoint = new Point(ActualWidth / 2, ActualHeight / 2);
            Position = TransformToAncestor(parentGraphControl).Transform(centerPoint);
        }

        #region Mouse input

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            ParentElementItem.Focus();

            _lastMousePosition = e.GetPosition(ParentGraphControl);
            e.Handled = true;

            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_isDragging)
                {
                    var currentMousePosition = e.GetPosition(ParentGraphControl);
                    var offset = currentMousePosition - _lastMousePosition;

                    _lastMousePosition = currentMousePosition;

                    RaiseEvent(new ConnectorItemDraggingEventArgs(ConnectorDraggingEvent, 
                        this, offset.X, offset.Y));
                }
                else
                {
                    var eventArgs = new ConnectorItemDragStartedEventArgs(ConnectorDragStartedEvent, this);
                    RaiseEvent(eventArgs);

                    if (eventArgs.Cancel)
                        return;

                    _isDragging = true;
                    CaptureMouse();
                }

                e.Handled = true;
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                RaiseEvent(new ConnectorItemDragCompletedEventArgs(ConnectorDragCompletedEvent, this));
                ReleaseMouseCapture();
                _isDragging = false;
                e.Handled = true;
            }
            base.OnMouseLeftButtonUp(e);
        }

        #endregion
    }
}