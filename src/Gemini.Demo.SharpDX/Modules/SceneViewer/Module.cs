using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Demo.SharpDX.Modules.SceneViewer.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Modules.MainMenu.Models;

namespace Gemini.Demo.SharpDX.Modules.SceneViewer
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
	    public override IEnumerable<IDocument> DefaultDocuments
	    {
            get { yield return new SceneViewModel(); }
	    }

	    public override void Initialize()
		{
			MainMenu.All
				.First(x => x.Name == "View")
				.Add(new MenuItem("3D Scene", OpenScene));
		}

		public override void PostInitialize()
		{
			if (!Shell.Documents.Any(x => x is SceneViewModel))
				Shell.OpenDocument(new SceneViewModel());

			base.PostInitialize();
		}

		private IEnumerable<IResult> OpenScene()
		{
            yield return Show.Document(new SceneViewModel());
		}
	}
}