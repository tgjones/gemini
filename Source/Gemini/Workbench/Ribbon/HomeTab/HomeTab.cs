using System.Collections.Generic;
using System.ComponentModel.Composition;
using Gemini.Contracts;
using Gemini.Contracts.Gui.Ribbon.RibbonGroup;
using Gemini.Contracts.Gui.Ribbon.RibbonTab;

namespace Gemini.Workbench.Ribbon.HomeTab
{
	[Export(ContractNames.ExtensionPoints.Workbench.Ribbon.Tabs, typeof(IRibbonTab))]
	public class HomeTab : AbstractRibbonTab
	{
		public HomeTab()
		{
			Title = "Home";
		}

		[ImportMany(ContractNames.ExtensionPoints.Workbench.Ribbon.HomeTab, typeof(IRibbonGroup), AllowRecomposition = true)]
		protected override IEnumerable<IRibbonGroup> GroupsInternal { get; set; }
	}
}