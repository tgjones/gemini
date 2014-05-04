using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;

namespace Gemini.Modules.MainMenu.Models
{
	public class MenuItemBase : PropertyChangedBase, IEnumerable<MenuItemBase>
	{
		#region Static stuff

		public static MenuItemBase Separator
		{
			get { return new MenuItemSeparator(); }
		}

		#endregion

		#region Properties

		public IObservableCollection<MenuItemBase> Children { get; private set; }

        public virtual string Name {
            get;
            private set;
        }

		#endregion

		#region Constructors

		protected MenuItemBase(string name = null)
		{
			Children = new BindableCollection<MenuItemBase>();
            this.Name = name ?? "-";
		}

		#endregion

		public void Add(params MenuItemBase[] menuItems)
		{
			menuItems.Apply(Children.Add);
		}

		public IEnumerator<MenuItemBase> GetEnumerator()
		{
			return Children.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public MenuItemBase this[string key]
		{
			get
			{
				return Children.FirstOrDefault(x => x.Name == key);
			}
		}

		public bool Remove(string name)
		{
			return Children.Remove(this[name]);
		}
	}
}