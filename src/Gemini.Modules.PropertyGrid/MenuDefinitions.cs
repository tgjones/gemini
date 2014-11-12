using System.ComponentModel.Composition;
using Gemini.Framework.Menus;
using Gemini.Modules.PropertyGrid.Commands;

namespace Gemini.Modules.PropertyGrid
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition ViewPropertyGridMenuItem = new CommandMenuItemDefinition<ViewPropertyGridCommandDefinition>(
            MainMenu.MenuDefinitions.ViewPropertiesMenuGroup, 0);
    }
}