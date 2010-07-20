using System;
using System.ComponentModel.Composition;
using System.Windows;
using Gemini.Contracts;
using Gemini.Contracts.Application.Startup;
using Gemini.Contracts.Gui.Layout;
using Gemini.Contracts.Services.ExtensionService;
using Gemini.Contracts.Services.OutputService;
using Gemini.Contracts.Services.PropertyGridService;

namespace Gemini.Demo.Application.StartupCommands
{
	/// <summary>
	/// This just makes sure that when we startup, the Pin Ball Table is always visible
	/// </summary>
	[Export(ContractNames.ExtensionPoints.Host.StartupCommands, typeof(IExecutableCommand))]
	public class StartupCommand : AbstractExtension, IExecutableCommand
	{
		[Import(ContractNames.CompositionPoints.Host.MainWindow, typeof(Window))]
		private Lazy<Window> MainWindow { get; set; }

		[Import(ContractNames.Services.Layout.LayoutManager, typeof(ILayoutManager))]
		private Lazy<ILayoutManager> LayoutManager { get; set; }

		[Import(DemoContractNames.CompositionPoints.PinBall.PinBallTable, typeof(DemoDocument.DemoDocument))]
		private Lazy<DemoDocument.DemoDocument> DemoDocument { get; set; }

		[Import(ContractNames.Services.PropertyGrid.PropertyGridService, typeof(IPropertyGridService))]
		private Lazy<IPropertyGridService> PropertyGridService { get; set; }

		[Import(ContractNames.Services.Output.OutputService, typeof(IOutputService))]
		private Lazy<IOutputService> OutputService { get; set; }

		public void Run(params object[] args)
		{
			MainWindow.Value.Title = "Osiris Demo";

			LayoutManager.Value.ShowDocument(DemoDocument.Value, string.Empty); // Makes sure it's shown every time

			PropertyGridService.Value.SelectedObject = DemoDocument.Value.Camera;

			OutputService.Value.Append("Started up");

			/*// Sorry, this is a really hacky way of setting the icon on the
			// main window, and only because I can't seem to convert from
			// a PNG to an icon any other way.
			Dummy dummy = new Dummy();
			mainWindow.Value.Icon = dummy.Icon;
			dummy.Close();*/
		}
	}
}