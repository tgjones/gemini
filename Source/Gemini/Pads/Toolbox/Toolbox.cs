using System.ComponentModel.Composition;
using Gemini.Contracts;
using Gemini.Contracts.Gui.Layout;

namespace Gemini.Pads.Toolbox
{
	[Export(ContractNames.ExtensionPoints.Workbench.Pads, typeof(IPad))]
	[Export(ContractNames.CompositionPoints.Workbench.Pads.Toolbox, typeof(Toolbox))]
	[Pad(Name = PadName)]
	public class Toolbox : AbstractPad
	{
		public const string PadName = "Toolbox";

		public Toolbox()
		{
			Name = PadName;
			Title = "Toolbox";
		}

		public object SelectedObject { get; set; }
	}
}