using System.Collections.Generic;
using Gemini.Contracts.Gui.Ribbon.RibbonGroup;

namespace Gemini.Contracts.Gui.Ribbon.RibbonTab
{
	public interface IRibbonTab : IRibbonItem
	{
		string Title { get; }
		IEnumerable<IRibbonGroup> Groups { get; }
	}
}