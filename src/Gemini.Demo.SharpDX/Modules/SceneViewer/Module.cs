using System.Collections.Generic;
using System.ComponentModel.Composition;
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
	}
}