using System.Collections.Generic;
using Caliburn.Micro;
using Gemini.Modules.MainMenu.Models;

namespace Gemini.Modules.MainMenu
{
	public interface IMenu : IObservableCollection<MenuItemBase>
	{
		IEnumerable<MenuItemBase> All { get; }
        //MenuItemBase Find(string name);
        //bool Remove(string name);
	}
}