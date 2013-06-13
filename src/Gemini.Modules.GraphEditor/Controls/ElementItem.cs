using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gemini.Modules.GraphEditor.Util;

namespace Gemini.Modules.GraphEditor.Controls
{
    public class ElementItem : ListBoxItem
    {
        private Point _lastMousePosition;
        private bool _isLeftMouseButtonDown;
        private bool _isDragging;

        static ElementItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ElementItem),
                new FrameworkPropertyMetadata(typeof(ElementItem)));
        }

        public static readonly DependencyProperty XProperty = DependencyProperty.Register(
            "X", typeof(double), typeof(ElementItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double X
        {
            get { return (double) GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        public static readonly DependencyProperty YProperty = DependencyProperty.Register(
            "Y", typeof(double), typeof(ElementItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double Y
        {
            get { return (double) GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        private ElementItemsControl ParentElementItemsControl
        {
            get { return VisualTreeUtility.FindParent<ElementItemsControl>(this); }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            DoSelection();

            var parentElementItemsControl = ParentElementItemsControl;
            if (parentElementItemsControl != null)
                _lastMousePosition = e.GetPosition(parentElementItemsControl);

            _isLeftMouseButtonDown = true;

            e.Handled = true;

            base.OnMouseLeftButtonDown(e);
        }

        private void DoSelection()
        {
            var parentElementItemsControl = ParentElementItemsControl;
            if (parentElementItemsControl == null)
                return;

            parentElementItemsControl.SelectedItems.Clear();
            IsSelected = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_isDragging)
            {
                var newMousePosition = e.GetPosition(ParentElementItemsControl);
                var delta = newMousePosition - _lastMousePosition;

                X += delta.X;
                Y += delta.Y;

                _lastMousePosition = newMousePosition;
            }
            if (_isLeftMouseButtonDown)
            {
                _isDragging = true;
                CaptureMouse();
            }

            e.Handled = true;

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_isLeftMouseButtonDown)
            {
                _isLeftMouseButtonDown = false;

                if (_isDragging)
                {
                    ReleaseMouseCapture();
                    _isDragging = false;
                }
            }

            e.Handled = true;

            base.OnMouseLeftButtonUp(e);
        }
    }
}