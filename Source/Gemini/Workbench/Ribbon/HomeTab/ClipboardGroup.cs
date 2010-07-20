using System.Collections.Generic;
using System.ComponentModel.Composition;
using Gemini.Contracts;
using Gemini.Contracts.Gui.Ribbon;
using Gemini.Contracts.Gui.Ribbon.RibbonGroup;

namespace Gemini.Workbench.Ribbon.HomeTab
{
	[Export(ContractNames.ExtensionPoints.Workbench.Ribbon.HomeTab, typeof(IRibbonGroup))]
	public class ClipboardGroup : AbstractRibbonGroup
	{
		public ClipboardGroup()
		{
			Title = "Clipboard";
		}

		[ImportMany(ContractNames.ExtensionPoints.Workbench.Ribbon.HomeTabGroups.ClipboardGroup, typeof(IRibbonItem), AllowRecomposition = true)]
		protected override IEnumerable<IRibbonItem> ItemsInternal { get; set; }
	}
}