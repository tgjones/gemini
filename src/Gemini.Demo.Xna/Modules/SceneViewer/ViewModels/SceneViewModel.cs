using System.ComponentModel.Composition;
using Gemini.Framework;

namespace Gemini.Demo.Xna.Modules.SceneViewer.ViewModels
{
	[Export(typeof(SceneViewModel))]
	public class SceneViewModel : Document
	{
		public override string DisplayName
		{
			get { return "3D Scene"; }
		}
	}
}