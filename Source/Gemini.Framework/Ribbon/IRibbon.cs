using System.Collections.Generic;

namespace Gemini.Framework.Ribbon
{
	public interface IRibbon
	{
		IEnumerable<IRibbonBackstageItem> BackstageItems { get; }
		IEnumerable<IRibbonTab> Tabs { get; }

		void AddBackstageItems(params IRibbonBackstageItem[] backstageItems);
		void AddTabs(params IRibbonTab[] tabs);
	}
}