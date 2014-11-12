using System.ComponentModel.Composition;
using Gemini.Framework.Menus;
using Gemini.Modules.UndoRedo.Commands;

namespace Gemini.Modules.UndoRedo
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition EditUndoMenuItem = new CommandMenuItemDefinition<UndoCommandDefinition>(
            MainMenu.MenuDefinitions.EditUndoRedoMenuGroup, 0);

        [Export]
        public static MenuItemDefinition EditRedoMenuItem = new CommandMenuItemDefinition<RedoCommandDefinition>(
            MainMenu.MenuDefinitions.EditUndoRedoMenuGroup, 1);

        [Export]
        public static MenuItemDefinition ViewHistoryMenuItem = new CommandMenuItemDefinition<ViewHistoryCommandDefinition>(
            MainMenu.MenuDefinitions.ViewToolsMenuGroup, 5);
    }
}