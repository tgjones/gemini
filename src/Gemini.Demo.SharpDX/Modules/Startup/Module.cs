using System.ComponentModel.Composition;
using Gemini.Framework;

namespace Gemini.Demo.SharpDX.Modules.Startup
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
		public override void Initialize()
		{
			MainWindow.Title = "Gemini SharpDX Demo";
		}
	}
}