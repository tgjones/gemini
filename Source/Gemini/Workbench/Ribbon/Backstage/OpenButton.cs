using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Gemini.Contracts;
using Gemini.Contracts.Gui.Ribbon;
using Gemini.Contracts.Gui.Ribbon.RibbonButton;

namespace Gemini.Workbench.Ribbon.Backstage
{
	[Export(ContractNames.ExtensionPoints.Workbench.Ribbon.Backstage, typeof(IRibbonItem))]
	public class OpenButton : AbstractRibbonButton
	{
		public OpenButton()
		{
			ID = "Open";
			Text = "Open";
			Icon = new BitmapImage(new Uri("pack://application:,,,/Gemini;component/Workbench/Resources/OpenFolder.png"));

			BeforeOrAfter = Contracts.Services.ExtensionService.RelativeDirection.After;
			InsertRelativeToID = "Save";
		}

		protected override ICommand CreateCommand()
		{
			return ApplicationCommands.Open;
		}
	}
}