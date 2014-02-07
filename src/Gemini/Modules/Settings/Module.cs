using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
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
		    MenuItemBase toolsMenu = MainMenu.All.FirstOrDefault(x => x.Name == "_Tools");

		    if (toolsMenu == null)
		    {
		        toolsMenu = new MenuItem("_Tools");
                MainMenu.Add(toolsMenu);
		    }

		    toolsMenu.Add(new MenuItem("_Options", OpenSettings));
		}

		private IEnumerable<IResult> OpenSettings()
		{
			yield return Show.Dialog<SettingsViewModel>();
		}
	}
}