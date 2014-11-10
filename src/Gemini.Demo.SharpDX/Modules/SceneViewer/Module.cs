using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Gemini.Demo.SharpDX.Modules.SceneViewer.ViewModels;
using Gemini.Framework;

namespace Gemini.Demo.SharpDX.Modules.SceneViewer
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
	    public override IEnumerable<IDocument> DefaultDocuments
	    {
            get { yield return new SceneViewModel(); }
	    }

		public override void PostInitialize()
		{
			if (!Shell.Documents.Any(x => x is SceneViewModel))
				Shell.OpenDocument(new SceneViewModel());

			base.PostInitialize();
		}
	}
}