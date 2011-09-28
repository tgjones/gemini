using System.Collections.Generic;
using Caliburn.Micro;

namespace Gemini.Framework.Menus
{
	public class MenuModel : BindableCollection<MenuItem>, IMenu
	{
		public IEnumerable<MenuItem> All
		{
			get
			{
				var queue = new Queue<MenuItem>(this);
				while (queue.Count > 0)
				{
					var current = queue.Dequeue();
					foreach (var item in current)
						queue.Enqueue(item);
					yield return current;
				}
			}
		}

		public void Add(params MenuItem[] items)
		{
			items.Apply(Add);
		}
	}
}