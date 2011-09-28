using System.Collections.Generic;
using Caliburn.Micro;

namespace Gemini.Framework.Menus
{
	public interface IMenu : IObservableCollection<MenuItem>
	{
		IEnumerable<MenuItem> All { get; }
	}
}