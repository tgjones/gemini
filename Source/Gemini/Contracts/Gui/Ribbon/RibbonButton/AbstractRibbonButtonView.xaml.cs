using System.ComponentModel.Composition;
using System.Windows;

namespace Gemini.Contracts.Gui.Ribbon.RibbonButton
{
	[Export(ContractNames.ExtensionPoints.Host.Views, typeof(ResourceDictionary))]
	public partial class AbstractRibbonButtonView : ResourceDictionary
	{
		public AbstractRibbonButtonView()
		{
			InitializeComponent();
		}
	}
}
