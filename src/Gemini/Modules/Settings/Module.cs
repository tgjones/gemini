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
            MainMenu.All.First(x => x.Name == "View")
                .Add(new MenuItem("Settings", OpenSettings));
		}

		private IEnumerable<IResult> OpenSettings()
		{
			yield return Show.Window<SettingsViewModel>();
		}
	}
}