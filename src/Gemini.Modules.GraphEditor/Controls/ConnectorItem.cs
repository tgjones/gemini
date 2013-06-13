using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gemini.Modules.GraphEditor.Util;

namespace Gemini.Modules.GraphEditor.Controls
{
    public class ConnectorItem : ContentControl
    {
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

        private ElementItem ParentElementItem
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
            base.OnMouseLeftButtonDown(e);
        }

        #endregion
    }
}