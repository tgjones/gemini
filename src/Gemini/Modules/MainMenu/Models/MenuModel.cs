using System.Collections.Generic;
using Caliburn.Micro;

namespace Gemini.Modules.MainMenu.Models
{
	public class MenuModel : BindableCollection<MenuItemBase>, IMenu
	{
		public IEnumerable<MenuItemBase> All
		{
			get
			{
				var queue = new Queue<MenuItemBase>(this);
				while (queue.Count > 0)
				{
					var current = queue.Dequeue();
					foreach (var item in current)
						queue.Enqueue(item);
					yield return current;
				}
			}
		}

		public void Add(params MenuItemBase[] items)
		{
			items.Apply(Add);
		}
	}
}