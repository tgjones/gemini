using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gemini.Framework;
using Gemini.Modules.Toolbox.ViewModels;

namespace Gemini.Modules.Toolbox.Views
{
    /// <summary>
    /// Interaction logic for ToolboxView.xaml
    /// </summary>
    public partial class ToolboxView : UserControl
    {
        private bool _draggingItem;
        private Point _mouseStartPosition;

        public ToolboxView()
        {
            InitializeComponent();
        }

        private void OnListBoxPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var listBoxItem = VisualTreeUtility.FindParent<ListBoxItem>(
                (DependencyObject) e.OriginalSource);
            _draggingItem = listBoxItem != null;

            _mouseStartPosition = e.GetPosition(ListBox);
        }

        private void OnListBoxMouseMove(object sender, MouseEventArgs e)
        {
            if (!_draggingItem)
                return;

            // Get the current mouse position
            Point mousePosition = e.GetPosition(null);
            Vector diff = _mouseStartPosition - mousePosition;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var listBoxItem = VisualTreeUtility.FindParent<ListBoxItem>(
                    (DependencyObject) e.OriginalSource);

                if (listBoxItem == null)
                    return;

                var itemViewModel = (ToolboxItemViewModel) ListBox.ItemContainerGenerator.
                    ItemFromContainer(listBoxItem);

                var dragData = new DataObject(ToolboxDragDrop.DataFormat, itemViewModel.Model);
                DragDrop.DoDragDrop(listBoxItem, dragData, DragDropEffects.Move);
            }
        }
    }
}
