using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Demo.Modules.Home.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Gemini.Framework.Results;
using Gemini.Modules.Inspector;
using Gemini.Modules.Inspector.Inspectors;
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

		    _inspectorTool.SelectedObject = new InspectableObject(
		        new IInspector[]
		        {
		            new CollapsibleGroupViewModel("Left Panel", new IInspector[]
		            {
		                new CheckBoxEditorViewModel
		                {
		                    BoundPropertyDescriptor =
		                        BoundPropertyDescriptor.FromProperty(homeViewModel, "IsLeftPanelVisible")
		                },
		                new ColorEditorViewModel
		                {
		                    BoundPropertyDescriptor = BoundPropertyDescriptor.FromProperty(homeViewModel, "Background")
		                },
		                new ColorEditorViewModel
		                {
		                    BoundPropertyDescriptor = BoundPropertyDescriptor.FromProperty(homeViewModel, "Foreground")
		                }
		            }),
		            new CollapsibleGroupViewModel("Right Panel", new IInspector[]
		            {
		                new Point3DEditorViewModel
		                {
		                    BoundPropertyDescriptor = BoundPropertyDescriptor.FromProperty(homeViewModel, "CameraPosition")
		                },
		                new RangeEditorViewModel(1, 180)
		                {
		                    BoundPropertyDescriptor = BoundPropertyDescriptor.FromProperty(homeViewModel, "CameraFieldOfView")
		                },
		                new Point3DEditorViewModel
		                {
		                    BoundPropertyDescriptor = BoundPropertyDescriptor.FromProperty(homeViewModel, "LightPosition")
		                }
		            })
		        });
		}

		private IEnumerable<IResult> OpenHome()
		{
			yield return Show.Document<HomeViewModel>();
		}
	}
}