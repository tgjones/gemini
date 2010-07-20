using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Gemini.Contracts;

namespace Gemini.Pads.Output
{
	[Export(ContractNames.ExtensionPoints.Host.Views, typeof(ResourceDictionary))]
	public partial class OutputPadView : ResourceDictionary
	{
		private readonly OutputPad _viewModel;

		[ImportingConstructor]
		public OutputPadView(
			[Import(ContractNames.CompositionPoints.Workbench.Pads.Output)] OutputPad viewModel)
		{
			_viewModel = viewModel;
			InitializeComponent();
		}

		private void OutputTextBox_Loaded(object sender, RoutedEventArgs e)
		{
			_viewModel.TextChanged += (s2, e2) => ScrollToEnd(sender);
			ScrollToEnd(sender);
		}

		private static void ScrollToEnd(object sender)
		{
			((TextBox) sender).ScrollToEnd();
		}
	}
}
