using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Modules.MainMenu.Models;
using Gemini.Modules.Settings.ViewModels;

namespace Gemini.Modules.Settings
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
		public override void Initialize()
		{
		    var toolsMenu = MainMenu.Find(KnownMenuItemNames.Tools);

		    if (toolsMenu == null)
		    {
		        toolsMenu = new MenuItem(KnownMenuItemNames.Tools, "_Tools");
                MainMenu.Add(toolsMenu);
		    }

		    toolsMenu.Add(new MenuItem("_Options", OpenSettings));
		}

		private static IEnumerable<IResult> OpenSettings()
		{
			yield return Show.Dialog<SettingsViewModel>();
		}
	}
}