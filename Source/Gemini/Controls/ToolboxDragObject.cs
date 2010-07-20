using System.Windows;

namespace Gemini.Controls
{
	// Wraps info of the dragged object into a class
	public class ToolboxDragObject
	{
		// Xaml string that represents the serialized content
		public string Xaml { get; set; }

		// Defines width and height of the DesignerItem
		// when this DragObject is dropped on the DesignerCanvas
		public Size? DesiredSize { get; set; }
	}
}