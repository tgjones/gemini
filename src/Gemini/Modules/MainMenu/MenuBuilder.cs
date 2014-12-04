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
        private readonly MenuBarDefinition[] _menuBars;
        private readonly MenuDefinition[] _menus;
        private readonly MenuItemGroupDefinition[] _menuItemGroups;
        private readonly MenuItemDefinition[] _menuItems;

        [ImportingConstructor]
        public MenuBuilder(
            ICommandService commandService,
            [ImportMany] MenuBarDefinition[] menuBars,
            [ImportMany] MenuDefinition[] menus,
            [ImportMany] MenuItemGroupDefinition[] menuItemGroups,
            [ImportMany] MenuItemDefinition[] menuItems)
        {
            _commandService = commandService;
            _menuBars = menuBars;
            _menus = menus;
            _menuItemGroups = menuItemGroups;
            _menuItems = menuItems;
        }

        public void BuildMenuBar(MenuBarDefinition menuBarDefinition, MenuModel result)
        {
            var menus = _menus
                .Where(x => x.MenuBar == menuBarDefinition)
                .OrderBy(x => x.SortOrder);

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
            var groups = _menuItemGroups
                .Where(x => x.Parent == menu)
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
                    var menuItemModel = (menuItem.CommandDefinition != null)
                        ? new CommandMenuItem(_commandService.GetCommand(menuItem.CommandDefinition), menuModel)
                        : (StandardMenuItem) new TextMenuItem(menuItem);
                    AddGroupsRecursive(menuItem, menuItemModel);
                    menuModel.Add(menuItemModel);
                }

                if (i < groups.Count - 1 && menuItems.Any())
                    menuModel.Add(new MenuItemSeparator());
            }
        }
    }
}