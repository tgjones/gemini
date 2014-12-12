using System.ComponentModel.Composition;
using Gemini.Framework.Menus;
using Gemini.Modules.Shell.Commands;

namespace Gemini.Modules.Shell
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition FileNewMenuItem = new TextMenuItemDefinition(
            MainMenu.MenuDefinitions.FileNewOpenMenuGroup, 0, "_New");

        [Export]
        public static MenuItemGroupDefinition FileNewCascadeGroup = new MenuItemGroupDefinition(
            FileNewMenuItem, 0);

        [Export]
        public static MenuItemDefinition FileNewMenuItemList = new CommandMenuItemDefinition<NewFileCommandListDefinition>(
            FileNewCascadeGroup, 0);

        [Export]
        public static MenuItemDefinition FileOpenMenuItem = new CommandMenuItemDefinition<OpenFileCommandDefinition>(
            MainMenu.MenuDefinitions.FileNewOpenMenuGroup, 1);

        [Export]
        public static MenuItemDefinition FileExitMenuItem = new CommandMenuItemDefinition<ExitCommandDefinition>(
            MainMenu.MenuDefinitions.FileExitOpenMenuGroup, 0);

        [Export]
        public static MenuItemDefinition WindowDocumentList = new CommandMenuItemDefinition<SwitchToDocumentCommandListDefinition>(
            MainMenu.MenuDefinitions.WindowDocumentListMenuGroup, 0);
    }
}