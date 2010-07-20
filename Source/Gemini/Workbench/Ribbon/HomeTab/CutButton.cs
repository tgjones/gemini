using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Gemini.Contracts;
using Gemini.Contracts.Gui.Ribbon;
using Gemini.Contracts.Gui.Ribbon.RibbonButton;

namespace Gemini.Workbench.Ribbon.HomeTab
{
	[Export(ContractNames.ExtensionPoints.Workbench.Ribbon.HomeTabGroups.ClipboardGroup, typeof(IRibbonItem))]
	public class CutButton : AbstractRibbonButton
	{
		public CutButton()
		{
			ID = "Cut";
			Text = "Cut";
			Icon = new BitmapImage(new Uri("pack://application:,,,/Gemini;component/Workbench/Resources/Cut.png"));

			BeforeOrAfter = Contracts.Services.ExtensionService.RelativeDirection.After;
			InsertRelativeToID = "Paste";
		}

		protected override ICommand CreateCommand()
		{
			return ApplicationCommands.Cut;
		}
	}
}