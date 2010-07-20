using System.ComponentModel.Composition;
using System.Windows;

namespace Gemini.Contracts.Gui.Controls
{
	[Export(ContractNames.ExtensionPoints.Host.Views, typeof(ResourceDictionary))]
	public partial class AbstractButtonView : ResourceDictionary
	{
		public AbstractButtonView()
		{
			InitializeComponent();
		}
	}
}
