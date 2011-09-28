using System.Windows;
using System.Windows.Controls;

namespace Gemini.Framework.Menus
{
	public class MenuStyleSelector : StyleSelector
	{
		public override Style SelectStyle(object item, DependencyObject container)
		{
			if (((MenuItem) item).IsSeparator)
				return (Style) ((FrameworkElement) container).FindResource("menuSeparatorStyle");
			return (Style) ((FrameworkElement) container).FindResource("menuStyle");
		}
	}
}