using System.ComponentModel.Composition;
using System.Windows;
using Gemini.Contracts;

namespace Gemini.Demo.DemoDocument
{
	[Export(ContractNames.ExtensionPoints.Host.Views, typeof(ResourceDictionary))]
	public partial class DemoDocumentView : ResourceDictionary
	{
		public DemoDocumentView()
		{
			InitializeComponent();
		}
	}
}
