using System;
using System.ComponentModel.Composition;
using Gemini.Contracts;
using Gemini.Contracts.Gui.Layout;
using Gemini.Contracts.Services.PropertyGridService;
using Gemini.Pads.PropertyGrid;

namespace Gemini.Services.PropertyGridService
{
	[Export(ContractNames.Services.PropertyGrid.PropertyGridService, typeof(IPropertyGridService))]
	public class PropertyGridService : IPropertyGridService
	{
		[Import(ContractNames.Services.Layout.LayoutManager, typeof(ILayoutManager))]
		private Lazy<ILayoutManager> LayoutManager { get; set; }

		[Import(ContractNames.CompositionPoints.Workbench.Pads.PropertyGrid)]
		private PropertyGrid PropertyGrid { get; set; }

		public object SelectedObject
		{
			get { return PropertyGrid.SelectedObject; }
			set
			{
				PropertyGrid.SelectedObject = value;
				LayoutManager.Value.ShowPad(PropertyGrid);
			}
		}
	}
}