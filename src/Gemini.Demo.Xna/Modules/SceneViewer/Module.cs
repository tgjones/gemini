using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Demo.Xna.Modules.SceneViewer.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Gemini.Framework.Results;

namespace Gemini.Demo.Xna.Modules.SceneViewer
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
		public override void Initialize()
		{
			MainMenu.All
				.First(x => x.Name == "View")
				.Add(new MenuItem("3D Scene", OpenScene));

			Shell.OpenDocument(new SceneViewModel());
		}

		private IEnumerable<IResult> OpenScene()
		{
            yield return Show.Document(new SceneViewModel());
		}
	}
}