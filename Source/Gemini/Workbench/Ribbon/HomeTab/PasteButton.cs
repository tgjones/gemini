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
	public class PasteButton : AbstractRibbonButton
	{
		public PasteButton()
		{
			ID = "Paste";
			Text = "Paste";
			Icon = new BitmapImage(new Uri("pack://application:,,,/Gemini;component/Workbench/Resources/Paste.png"));
			LargeIcon = new BitmapImage(new Uri("pack://application:,,,/Gemini;component/Workbench/Resources/PasteBig.png"));
			SizeDefinition = "Large";
		}

		protected override ICommand CreateCommand()
		{
			return ApplicationCommands.Paste;
		}
	}
}