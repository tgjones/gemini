using System.Collections.Generic;

namespace Gemini.Contracts.Gui.Ribbon.RibbonGroup
{
	public interface IRibbonGroup : IRibbonItem
	{
		string Title { get; }
		IEnumerable<IRibbonItem> Items { get; }
	}
}