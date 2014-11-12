using System.ComponentModel.Composition;
using Gemini.Framework.Menus;
using Gemini.Modules.Inspector.Commands;

namespace Gemini.Modules.Inspector
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition ViewInspectorMenuItem = new CommandMenuItemDefinition<ViewInspectorCommandDefinition>(
            MainMenu.MenuDefinitions.ViewPropertiesMenuGroup, 1);
    }
}