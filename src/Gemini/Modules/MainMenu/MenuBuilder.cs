using System.ComponentModel.Composition;
using System.Linq;
using Gemini.Framework.Commands;
using Gemini.Framework.Menus;
using Gemini.Modules.MainMenu.Models;

namespace Gemini.Modules.MainMenu
{
    [Export(typeof(IMenuBuilder))]
    public class MenuBuilder : IMenuBuilder
    {
        private readonly ICommandService _commandService;
        private readonly MenuDefinition[] _menus;
        private readonly MenuItemGroupDefinition[] _menuItemGroups;
        private readonly MenuItemDefinition[] _menuItems;
        private readonly MenuDefinition[] _excludeMenus;
        private readonly MenuItemGroupDefinition[] _excludeMenuItemGroups;
        private readonly MenuItemDefinition[] _excludeMenuItems;

        [ImportingConstructor]
        public MenuBuilder(
            ICommandService commandService,
            [ImportMany] MenuDefinition[] menus,
            [ImportMany] MenuItemGroupDefinition[] menuItemGroups,
            [ImportMany] MenuItemDefinition[] menuItems,
            [ImportMany] ExcludeMenuDefinition[] excludeMenus,
            [ImportMany] ExcludeMenuItemGroupDefinition[] excludeMenuItemGroups,
            [ImportMany] ExcludeMenuItemDefinition[] excludeMenuItems)
        {
            _commandService = commandService;
            _menus = menus;
            _menuItemGroups = menuItemGroups;
            _menuItems = menuItems;
            _excludeMenus = excludeMenus.Select(x => x.MenuDefinitionToExclude).ToArray();
            _excludeMenuItemGroups = excludeMenuItemGroups.Select(x => x.MenuItemGroupDefinitionToExclude).ToArray();
            _excludeMenuItems = excludeMenuItems.Select(x => x.MenuItemDefinitionToExclude).ToArray();
        }

        public void BuildMenuBar(MenuBarDefinition menuBarDefinition, MenuModel result)
        {
            var menus =
                from menu in _menus
                where menu.MenuBar == menuBarDefinition
                where !IsExcluded(menu)
                orderby menu.SortOrder
                select menu;

            foreach (var menu in menus)
            {
                var menuModel = new TextMenuItem(menu);
                AddGroupsRecursive(menu, menuModel);
                if (menuModel.Children.Any())
                    result.Add(menuModel);
            }
        }

        private void AddGroupsRecursive(MenuDefinitionBase menu, StandardMenuItem menuModel)
        {
            var groupsExpression =
                from menuGroup in _menuItemGroups
                where menuGroup.Parent == menu
                where !IsExcluded(menuGroup)
                orderby menuGroup.SortOrder
                select menuGroup;

            var groups = groupsExpression.ToList();

            for (int i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var menuItems =
                    from menuItem in _menuItems
                    where menuItem.Group == @group
                    where !IsExcluded(menuItem)
                    orderby menuItem.SortOrder
                    select menuItem;

                foreach (var menuItem in menuItems)
                {
                    var menuItemModel = (menuItem.CommandDefinition != null)
                        ? new CommandMenuItem(_commandService.GetCommand(menuItem.CommandDefinition), menuModel)
                        : (StandardMenuItem)new TextMenuItem(menuItem);
                    AddGroupsRecursive(menuItem, menuItemModel);
                    menuModel.Add(menuItemModel);
                }

                if (i < groups.Count - 1 && menuItems.Any())
                    menuModel.Add(new MenuItemSeparator());
            }
        }

        private bool IsExcluded(MenuDefinition item)
        {
            if (_excludeMenus.Contains(item))
            {
                return true;
            }

            if (item.DynamicExclusionPredicate != null &&
                item.DynamicExclusionPredicate(item))
            {
                return true;
            }

            return false;
        }

        private bool IsExcluded(MenuItemGroupDefinition item)
        {
            if (_excludeMenuItemGroups.Contains(item))
            {
                return true;
            }

            if (item.DynamicExclusionPredicate != null &&
                item.DynamicExclusionPredicate(item))
            {
                return true;
            }

            return false;
        }

        private bool IsExcluded(MenuItemDefinition item)
        {
            if (_excludeMenuItems.Contains(item))
            {
                return true;
            }

            if (item.DynamicExclusionPredicate != null &&
                item.DynamicExclusionPredicate(item))
            {
                return true;
            }

            return false;
        }
    }
}
