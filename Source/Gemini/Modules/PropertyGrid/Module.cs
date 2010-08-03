using System.Collections.Generic;
using System.Linq;
using Caliburn.Core;
using Caliburn.PresentationFramework;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Framework.Ribbon;
using Gemini.Framework.Services;
using Gemini.Modules.PropertyGrid.ViewModels;

namespace Gemini.Modules.PropertyGrid
{
	public class Module : ModuleBase
	{
		protected override IEnumerable<ComponentInfo> GetComponents()
		{
			yield return Singleton<IPropertyGrid, PropertyGridViewModel>();
		}

		protected override void Initialize()
		{
			Ribbon.Tabs
				.First(x => x.Name == "Home")
				.Groups.First(x => x.Name == "Tools")
				.Add(new RibbonButton("Properties", OpenProperties));
		}

		private static IEnumerable<IResult> OpenProperties()
		{
			yield return Show.Tool<IPropertyGrid>(Pane.Right);
		}
	}
}