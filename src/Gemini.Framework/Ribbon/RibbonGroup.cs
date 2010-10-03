using System.Collections.Generic;
using System.Linq;

namespace Gemini.Framework.Ribbon
{
	public class RibbonGroup : IRibbonGroup
	{
		private List<IRibbonItem> _items;

		public string Title { get; set; }

		public IEnumerable<IRibbonItem> Items
		{
			get { return _items; }
			set { _items = value.ToList(); }
		}

		public string Name
		{
			get { return string.IsNullOrEmpty(Title) ? null : Title.Replace("_", string.Empty); }
		}

		public RibbonGroup(string title, IEnumerable<IRibbonItem> items)
			: this(title)
		{
			Items = items;
		}

		public RibbonGroup(string title)
		{
			_items = new List<IRibbonItem>();
			Title = title;
		}

		public void Add(IRibbonItem item)
		{
			_items.Add(item);
		}
	}
}