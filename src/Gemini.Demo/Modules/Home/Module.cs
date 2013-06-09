using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Demo.Modules.Home.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Modules.Inspector;
using Gemini.Modules.MainMenu.Models;
using Gemini.Modules.PropertyGrid;

namespace Gemini.Demo.Modules.Home
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
		[Import]
		private IPropertyGrid _propertyGrid;

        [Import]
	    private IInspectorTool _inspectorTool;

		public override void Initialize()
		{
			MainMenu.All
				.First(x => x.Name == "View")
				.Add(new MenuItem("Home", OpenHome));

			var homeViewModel = IoC.Get<HomeViewModel>();
			Shell.OpenDocument(homeViewModel);

			_propertyGrid.SelectedObject = homeViewModel;

		    _inspectorTool.SelectedObject = new InspectableObjectBuilder()
                .WithCollapsibleGroup("Left Panel", b => b
                    .WithCheckBoxEditor(homeViewModel, x => x.IsLeftPanelVisible)
                    .WithColorEditor(homeViewModel, x => x.Background)
                    .WithColorEditor(homeViewModel, x => x.Foreground)
                    .WithEnumEditor(homeViewModel, x => x.TextAlignment))
                .WithCollapsibleGroup("Right Panel", b => b
                    .WithObject(homeViewModel))
		        .ToInspectableObject();
		}

		private IEnumerable<IResult> OpenHome()
		{
			yield return Show.Document<HomeViewModel>();
		}
	}
}