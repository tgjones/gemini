using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Demo.Modules.Home.ViewModels;
using Gemini.Demo.Modules.Settings.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Gemini.Framework.Results;
using Gemini.Modules.Inspector;
using Gemini.Modules.PropertyGrid;

namespace Gemini.Demo.Modules.Settings
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
		public override void Initialize()
		{
			MainMenu.All
				.First(x => x.Name == "View")
				.Add(new MenuItem("Settings", OpenSettings));
		}

		private IEnumerable<IResult> OpenSettings()
		{
			yield return Show.Window<SettingsViewModel>();
		}
	}
}