using System.Windows;
using System.Windows.Controls;
using Gemini.Modules.MainMenu.Models;

namespace Gemini.Modules.MainMenu.Controls
{
	public class MenuItemEx : System.Windows.Controls.MenuItem
	{
		private object _currentItem;

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			_currentItem = item;
			return base.IsItemItsOwnContainerOverride(item);
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return GetContainer(this, _currentItem);
		}

		internal static DependencyObject GetContainer(FrameworkElement frameworkElement, object item)
		{
			if (item is MenuItemSeparator)
				return new Separator { Style = (Style)frameworkElement.FindResource(SeparatorStyleKey) };

			string styleKey = (item is CheckableMenuItem) ? "CheckableMenuItem" : "MenuItem";
			return new MenuItemEx { Style = (Style)frameworkElement.FindResource(styleKey) };
		}
	}
}