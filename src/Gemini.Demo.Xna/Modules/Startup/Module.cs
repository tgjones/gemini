using System.ComponentModel.Composition;
using Gemini.Framework;
using Gemini.Framework.Menus;

namespace Gemini.Demo.Xna.Modules.Startup
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
        [Export]
        public static MenuDefinition DemosMenu = new MenuDefinition(
            Gemini.Modules.MainMenu.MenuDefinitions.MainMenuBar, 5, "De_mos");

        [Export]
        public static MenuItemGroupDefinition DemosMenuGroup = new MenuItemGroupDefinition(
            DemosMenu, 0);

		public override void Initialize()
		{
			MainWindow.Title = "Gemini XNA Demo";
		}
	}
}