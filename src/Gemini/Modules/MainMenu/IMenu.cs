using System.Collections.Generic;
using Caliburn.Micro;
using Gemini.Modules.MainMenu.Models;

namespace Gemini.Modules.MainMenu
{
	public interface IMenu : IObservableCollection<MenuItemBase>
	{
		IEnumerable<MenuItemBase> All { get; }
		MenuItemBase this[string key] { get; }
	    bool Remove(string name);
	}
}