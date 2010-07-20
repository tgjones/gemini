using System.ComponentModel.Composition;
using System.Windows;
using Gemini.Contracts;

namespace Gemini.Layout
{
	[Export(ContractNames.ExtensionPoints.Host.Views, typeof(ResourceDictionary))]
	public partial class LayoutManagerView : ResourceDictionary
	{
		public LayoutManagerView()
		{
			InitializeComponent();
		}
	}
}
