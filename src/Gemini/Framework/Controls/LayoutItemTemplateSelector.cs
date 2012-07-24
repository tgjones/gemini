using System.Windows;
using System.Windows.Controls;

namespace Gemini.Framework.Controls
{
	public class LayoutItemTemplateSelector : DataTemplateSelector
	{
		public DataTemplate Template { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			return Template;
		}
	}
}