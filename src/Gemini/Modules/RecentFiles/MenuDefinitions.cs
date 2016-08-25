using Gemini.Framework.Menus;
using Gemini.Modules.RecentFiles.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemini.Modules.RecentFiles
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition FileRecentFilesMenuItem = new CommandMenuItemDefinition<RecentFilesCommandDefinition>(
            MainMenu.MenuDefinitions.FileOpenRecentMenuGroup, 0);

        [Export]
        public static MenuItemGroupDefinition FileRecentFilesCascadeGroup = new MenuItemGroupDefinition(
            FileRecentFilesMenuItem, 0);

        [Export]
        public static MenuItemDefinition FileOpenRecentMenuItemList = new CommandMenuItemDefinition<OpenRecentFileCommandListDefinition>(
            FileRecentFilesCascadeGroup, 0);
    }
}