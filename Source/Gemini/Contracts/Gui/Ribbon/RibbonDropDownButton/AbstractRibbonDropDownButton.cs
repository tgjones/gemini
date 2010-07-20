using System.Collections.Generic;
using Gemini.Contracts.Gui.Ribbon.RibbonButton;

namespace Gemini.Contracts.Gui.Ribbon.RibbonDropDownButton
{
	public abstract class AbstractRibbonDropDownButton : AbstractRibbonButton, IRibbonButton
	{
		public IEnumerable<IRibbonItem> Items { get; set; }
	}
}