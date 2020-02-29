using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Gemini.Framework;

namespace Gemini.Modules.GraphEditor.Controls
{
    public class ConnectionItem : ListBoxItem
    {
        static ConnectionItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConnectionItem),
                new FrameworkPropertyMetadata(typeof(ConnectionItem)));
        }

        private GraphControl ParentGraphControl
        {
            get { return VisualTreeUtility.FindParent<GraphControl>(this); }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            ParentGraphControl.UnselectAll();
            base.OnMouseLeftButtonDown(e);
        }
    }
}
