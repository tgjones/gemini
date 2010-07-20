using System.ComponentModel.Composition;
using Gemini.Contracts;
using Gemini.Contracts.Gui.Layout;

namespace Gemini.Pads.PropertyGrid
{
	[Export(ContractNames.ExtensionPoints.Workbench.Pads, typeof(IPad))]
	[Export(ContractNames.CompositionPoints.Workbench.Pads.PropertyGrid, typeof(PropertyGrid))]
	[Pad(Name = PadName)]
	public class PropertyGrid : AbstractPad
	{
		public const string PadName = "PropertyGrid";

		public PropertyGrid()
		{
			Name = PadName;
			Title = "Properties";
		}

		public object SelectedObject { get; set; }
	}
}