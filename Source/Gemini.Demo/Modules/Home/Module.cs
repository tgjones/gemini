using System.Collections.Generic;
using System.Linq;
using Caliburn.Core;
using Caliburn.PresentationFramework;
using Gemini.Demo.Modules.Home.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Framework.Ribbon;

namespace Gemini.Demo.Modules.Home
{
	public class Module : ModuleBase
	{
		protected override IEnumerable<ComponentInfo> GetComponents()
		{
			yield return PerRequest<HomeViewModel, HomeViewModel>();
		}

		protected override void Initialize()
		{
			Ribbon.Tabs
				.First(x => x.Name == "Home")
				.Groups.First(x => x.Name == "Tools")
				.Add(new RibbonButton("Home", OpenHome));

			Shell.OpenDocument(Container.GetInstance<HomeViewModel>());
		}

		private static IEnumerable<IResult> OpenHome()
		{
			yield return Show.Document<HomeViewModel>();
		}
	}
}