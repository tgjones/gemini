using System.Collections.Generic;
using System.Linq;
using Caliburn.Core;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Framework.Ribbon;
using Gemini.Framework.Services;
using Gemini.Modules.Output.ViewModels;
using Caliburn.PresentationFramework;

namespace Gemini.Modules.Output
{
	public class Module : ModuleBase
	{
		protected override IEnumerable<ComponentInfo> GetComponents()
		{
			yield return Singleton<IOutput, OutputViewModel>();
		}

		protected override void Initialize()
		{
			Ribbon.Tabs
				.First(x => x.Name == "Home")
				.Groups.First(x => x.Name == "Tools")
				.Add(new RibbonButton("Output", OpenOutput));
		}

		private static IEnumerable<IResult> OpenOutput()
		{
			yield return Show.Tool<IOutput>(Pane.Bottom);
		}
	}
}