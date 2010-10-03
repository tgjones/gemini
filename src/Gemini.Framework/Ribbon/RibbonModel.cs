using System.Collections.Generic;

namespace Gemini.Framework.Ribbon
{
	public class RibbonModel : IRibbon
	{
		private readonly List<IRibbonBackstageItem> _backstageItems;
		private readonly List<IRibbonTab> _tabs;

		public IEnumerable<IRibbonBackstageItem> BackstageItems
		{
			get { return _backstageItems; }
		}

		public IEnumerable<IRibbonTab> Tabs
		{
			get { return _tabs; }
		}

		public RibbonModel()
		{
			_backstageItems = new List<IRibbonBackstageItem>();
			_tabs = new List<IRibbonTab>();
		}

		public void AddBackstageItems(params IRibbonBackstageItem[] backstageItems)
		{
			_backstageItems.AddRange(backstageItems);
		}

		public void AddTabs(params IRibbonTab[] tabs)
		{
			_tabs.AddRange(tabs);
		}
	}
}