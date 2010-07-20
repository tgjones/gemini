using System.Drawing.Design;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Gemini.Controls
{
	public class ToolboxItemControl : ContentControl
	{
		// caches the start point of the drag operation
		private Point? _dragStartPoint = null;

		static ToolboxItemControl()
		{
			// set the key to reference the style for this control
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
					typeof(ToolboxItem), new FrameworkPropertyMetadata(typeof(ToolboxItem)));
		}

		protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseDown(e);
			_dragStartPoint = new Point?(e.GetPosition(this));
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.LeftButton != MouseButtonState.Pressed)
				_dragStartPoint = null;

			if (_dragStartPoint.HasValue)
			{
				// XamlWriter.Save() has limitations in exactly what is serialized,
				// see SDK documentation; short term solution only;
				string xamlString = XamlWriter.Save(this.Content);
				ToolboxDragObject dataObject = new ToolboxDragObject();
				dataObject.Xaml = xamlString;

				WrapPanel panel = VisualTreeHelper.GetParent(this) as WrapPanel;
				if (panel != null)
				{
					// desired size for DesignerCanvas is the stretched Toolbox item size
					double scale = 1.3;
					dataObject.DesiredSize = new Size(panel.ItemWidth * scale, panel.ItemHeight * scale);
				}

				DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);

				e.Handled = true;
			}
		}
	}
}