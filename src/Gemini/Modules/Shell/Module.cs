using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Gemini.Framework;
using Gemini.Modules.MainMenu.Models;

namespace Gemini.Modules.Shell
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
		public override void Initialize()
		{
			((StandardMenuItem) MainMenu.All.First(x => x.Name == "Open"))
				.WithGlobalShortcut(ModifierKeys.Control, Key.O);
		}
	}
}