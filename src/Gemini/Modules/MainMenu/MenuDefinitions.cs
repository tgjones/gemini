using System.ComponentModel.Composition;
using Gemini.Framework.Menus;
using Gemini.Properties;

namespace Gemini.Modules.MainMenu
{
    public static class MenuDefinitions
    {
        [Export]
        public static readonly MenuBarDefinition MainMenuBar = new MenuBarDefinition();

        [Export]
        public static readonly MenuDefinition FileMenu = new MenuDefinition(MainMenuBar, 0, Resources.FileMenuText);

        [Export]
        public static readonly MenuItemGroupDefinition FileNewOpenMenuGroup = new MenuItemGroupDefinition(FileMenu, 0);

        [Export]
        public static readonly MenuItemGroupDefinition FileCloseMenuGroup = new MenuItemGroupDefinition(FileMenu, 3);

        [Export]
        public static readonly MenuItemGroupDefinition FileSaveMenuGroup = new MenuItemGroupDefinition(FileMenu, 6);

        [Export]
        public static readonly MenuItemGroupDefinition FileExitOpenMenuGroup = new MenuItemGroupDefinition(FileMenu, 10);

        [Export]
        public static readonly MenuDefinition EditMenu = new MenuDefinition(MainMenuBar, 1, Resources.EditMenuText);

        [Export]
        public static readonly MenuItemGroupDefinition EditUndoRedoMenuGroup = new MenuItemGroupDefinition(EditMenu, 0);

        [Export]
        public static readonly MenuDefinition ViewMenu = new MenuDefinition(MainMenuBar, 2, Resources.ViewMenuText);

        [Export]
        public static readonly MenuItemGroupDefinition ViewToolsMenuGroup = new MenuItemGroupDefinition(ViewMenu, 0);

        [Export]
        public static readonly MenuItemGroupDefinition ViewPropertiesMenuGroup = new MenuItemGroupDefinition(ViewMenu, 100);

        [Export]
        public static readonly MenuDefinition ToolsMenu = new MenuDefinition(MainMenuBar, 10, Resources.ToolsMenuText);

        [Export]
        public static readonly MenuItemGroupDefinition ToolsOptionsMenuGroup = new MenuItemGroupDefinition(ToolsMenu, 100);

        [Export]
        public static readonly MenuDefinition WindowMenu = new MenuDefinition(MainMenuBar, 20, Resources.WindowMenuText);

        [Export]
        public static readonly MenuItemGroupDefinition WindowDocumentListMenuGroup = new MenuItemGroupDefinition(WindowMenu, 10);

        [Export]
        public static readonly MenuDefinition HelpMenu = new MenuDefinition(MainMenuBar, 30, Resources.HelpMenuText);
    }
}
