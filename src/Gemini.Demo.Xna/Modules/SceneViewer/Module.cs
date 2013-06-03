using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Demo.Xna.Modules.SceneViewer.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Modules.Inspector;
using Gemini.Modules.Inspector.Xna;
using Gemini.Modules.MainMenu.Models;

namespace Gemini.Demo.Xna.Modules.SceneViewer
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
	    private readonly IInspectorTool _inspectorTool;

        [ImportingConstructor]
	    public Module(IInspectorTool inspectorTool)
        {
            _inspectorTool = inspectorTool;
        }

	    public override void Initialize()
		{
			MainMenu.All
				.First(x => x.Name == "View")
				.Add(new MenuItem("3D Scene", OpenScene));

	        var viewModel = new SceneViewModel();
			Shell.OpenDocument(viewModel);

	        _inspectorTool.SelectedObject = new InspectableObjectBuilder()
                .WithVector3Editor(viewModel, x => x.Position)
	            .ToInspectableObject();
		}

		private IEnumerable<IResult> OpenScene()
		{
            yield return Show.Document(new SceneViewModel());
		}
	}
}