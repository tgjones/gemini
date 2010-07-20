using System;
using System.ComponentModel.Composition;
using Gemini.Contracts;
using Gemini.Contracts.Gui.Layout;
using Gemini.Contracts.Services.OutputService;
using Gemini.Pads.Output;

namespace Gemini.Services.OutputService
{
	[Export(ContractNames.Services.Output.OutputService, typeof(IOutputService))]
	public class OutputService : IOutputService
	{
		[Import(ContractNames.Services.Layout.LayoutManager, typeof(ILayoutManager))]
		private Lazy<ILayoutManager> LayoutManager { get; set; }

		[Import(ContractNames.CompositionPoints.Workbench.Pads.Output)]
		private OutputPad OutputPad { get; set; }

		public void Append(string text)
		{
			OutputPad.AppendText(text);
			ShowOutputPad();
		}

		public void Clear()
		{
			OutputPad.ClearText();
			ShowOutputPad();
		}

		private void ShowOutputPad()
		{
			if (!LayoutManager.Value.IsVisible(OutputPad))
				LayoutManager.Value.ShowPad(OutputPad, AvalonDock.AnchorStyle.Bottom);
		}
	}
}