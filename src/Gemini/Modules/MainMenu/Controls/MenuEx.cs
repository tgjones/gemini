using System.Windows;

namespace Gemini.Modules.MainMenu.Controls
{
	public class MenuEx : System.Windows.Controls.Menu
	{
		private object _currentItem;

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			_currentItem = item;
			return base.IsItemItsOwnContainerOverride(item);
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return MenuItemEx.GetContainer(this, _currentItem);
		}
	}
}