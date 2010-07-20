using System.Windows;
using System.Windows.Controls;

namespace Gemini.Controls
{
	public class ToolboxCategoryControl : ItemsControl
	{
		public Size ItemSize { get; set; }

		public ToolboxCategoryControl()
		{
			ItemSize = new Size(50, 50);
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new ToolboxItemControl();
		}

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return (item is ToolboxItemControl);
		}
	}
}