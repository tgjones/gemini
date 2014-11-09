using System.ComponentModel.Composition;
using Gemini.Framework.Menus;

namespace Gemini.Modules.MainMenu
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuBarDefinition MainMenuBar = new MenuBarDefinition();

        [Export]
        public static MenuDefinition FileMenu = new MenuDefinition(MainMenuBar, 0, "_File");

        [Export]
        public static MenuItemGroupDefinition FileNewOpenMenuGroup = new MenuItemGroupDefinition(FileMenu, 0);

        [Export]
        public static MenuItemGroupDefinition FileExitOpenMenuGroup = new MenuItemGroupDefinition(FileMenu, 10);
    }
}