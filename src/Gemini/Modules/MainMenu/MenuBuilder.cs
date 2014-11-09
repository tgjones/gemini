using System.ComponentModel.Composition;
using System.Linq;
using Gemini.Framework.Menus;
using Gemini.Modules.MainMenu.Models;

namespace Gemini.Modules.MainMenu
{
    [Export(typeof(IMenuBuilder))]
    public class MenuBuilder : IMenuBuilder
    {
        [ImportMany]
        private MenuBarDefinition[] _menuBars;

        [ImportMany]
        private MenuDefinition[] _menus;

        [ImportMany]
        private MenuItemGroupDefinition[] _menuItemGroups;

        [ImportMany]
        private MenuItemDefinition[] _menuItems;

        public void BuildMenuBar(MenuBarDefinition menuBarDefinition, MenuModel result)
        {
            var menus = _menus
                .Where(x => x.MenuBar == menuBarDefinition)
                .OrderBy(x => x.SortOrder);

            foreach (var menu in menus)
            {
                var menuModel = new MenuItem(menu);
                AddGroupsRecursive(menu, menuModel);
                result.Add(menuModel);
            }
        }

        private void AddGroupsRecursive(MenuDefinitionBase menu, MenuItem menuModel)
        {
            var groups = _menuItemGroups
                .Where(x => x.Menu == menu)
                .OrderBy(x => x.SortOrder)
                .ToList();

            for (int i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var menuItems = _menuItems
                    .Where(x => x.Group == group)
                    .OrderBy(x => x.SortOrder);

                foreach (var menuItem in menuItems)
                {
                    var menuItemModel = new MenuItem(menuItem);
                    AddGroupsRecursive(menuItem, menuItemModel);
                    menuModel.Add(menuItemModel);
                }

                if (i < groups.Count - 1)
                    menuModel.Add(new MenuItemSeparator());
            }
        }
    }
}