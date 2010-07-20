using System.ComponentModel.Composition;
using System.Windows;
using Gemini.Contracts;

namespace Gemini.Pads.PropertyGrid
{
	[Export(ContractNames.ExtensionPoints.Host.Views, typeof(ResourceDictionary))]
	public partial class PropertyGridView : ResourceDictionary
	{
		public PropertyGridView()
		{
			InitializeComponent();
		}
	}
}
