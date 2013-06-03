using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Framework.Services;
using Gemini.Modules.MainMenu.Models;

namespace Gemini.Modules.PropertyGrid
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
		public override void Initialize()
		{
			MainMenu.All.First(x => x.Name == "View")
				.Add(new MenuItem("Properties", OpenProperties).WithIcon());
		}

		private static IEnumerable<IResult> OpenProperties()
		{
			yield return Show.Tool<IPropertyGrid>();
		}
	}
}