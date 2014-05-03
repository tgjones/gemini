using System.ComponentModel.Composition;
using Gemini.Framework;

namespace Gemini.Demo.MonoGame.Modules.Startup
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
		public override void Initialize()
		{
			MainWindow.Title = "Gemini MonoGame Demo";
		}
	}
}