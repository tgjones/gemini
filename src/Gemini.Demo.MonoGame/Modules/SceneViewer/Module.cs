using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Demo.MonoGame.Modules.SceneViewer.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Modules.Inspector;
using Gemini.Modules.Inspector.MonoGame;
using Gemini.Modules.MainMenu.Models;

namespace Gemini.Demo.MonoGame.Modules.SceneViewer
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
	    private readonly IInspectorTool _inspectorTool;

	    public override IEnumerable<IDocument> DefaultDocuments
	    {
            get { yield return new SceneViewModel(); }
	    }

	    [ImportingConstructor]
	    public Module(IInspectorTool inspectorTool)
        {
            _inspectorTool = inspectorTool;
        }

	    public override void Initialize()
		{
            //MainMenu.Find(KnownMenuItemNames.View).Add(new MenuItem("3D Scene", OpenScene));
		}

	    public override void PostInitialize()
	    {
            var sceneViewModel = Shell.Documents.OfType<SceneViewModel>().FirstOrDefault();
            if (sceneViewModel != null)
                _inspectorTool.SelectedObject = new InspectableObjectBuilder()
                    .WithVector3Editor(sceneViewModel, x => x.Position)
                    .ToInspectableObject();
	    }

	    private IEnumerable<IResult> OpenScene()
		{
            yield return Show.Document(new SceneViewModel());
		}
	}
}