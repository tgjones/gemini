using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Demo.Modules.Home.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Gemini.Framework.Results;
using Gemini.Framework.Services;
using Gemini.Modules.PropertyGrid;

namespace Gemini.Demo.Modules.Home
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
		[Import]
		private IPropertyGrid _propertyGrid;

		public override void Initialize()
		{
			MainMenu.All
				.First(x => x.Name == "View")
				.Add(new MenuItem("Home", OpenHome));

			var homeViewModel = IoC.Get<HomeViewModel>();
			Shell.OpenDocument(homeViewModel);

			_propertyGrid.SelectedObject = homeViewModel;
		}

		private IEnumerable<IResult> OpenHome()
		{
			yield return Show.Document<HomeViewModel>();
		}
	}
}