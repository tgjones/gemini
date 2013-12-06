using System.ComponentModel.Composition;
using Gemini.Framework;

namespace Gemini.Demo.Xna.Modules.Startup
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
		public override void Initialize()
		{
			MainWindow.Title = "Gemini XNA Demo";
		}
	}
}