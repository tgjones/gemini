using System.ComponentModel.Composition;
using Gemini.Framework.Menus;
using Gemini.Modules.Settings.Commands;

namespace Gemini.Modules.Settings
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition OpenSettingsMenuItem = new CommandMenuItemDefinition<OpenSettingsCommandDefinition>(
            MainMenu.MenuDefinitions.ToolsOptionsMenuGroup, 0);
    }
}