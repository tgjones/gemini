using System.ComponentModel.Composition;
using Gemini.Framework.Menus;
using Gemini.Modules.ErrorList.Commands;

namespace Gemini.Modules.ErrorList
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition ViewErrorListMenuItem = new CommandMenuItemDefinition<ViewErrorListCommandDefinition>(
            MainMenu.MenuDefinitions.ViewToolsMenuGroup, 0);
    }
}