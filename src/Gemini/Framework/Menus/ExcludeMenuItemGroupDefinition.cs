using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemini.Framework.Menus
{
    public class ExcludeMenuItemGroupDefinition
    {
        private MenuItemGroupDefinition _menuItemGroupDefinitionToExclude;
        public MenuItemGroupDefinition MenuItemGroupDefinitionToExclude 
        {
            get { return _menuItemGroupDefinitionToExclude; }
        }

        private string _menuItemGroupDefinitionToExcludeParentText;
        public string MenuItemGroupDefinitionToExcludeParentText
        {
            get { return _menuItemGroupDefinitionToExcludeParentText; }
        }

        public ExcludeMenuItemGroupDefinition(MenuItemGroupDefinition menuItemGroupDefinition)
        {
            _menuItemGroupDefinitionToExclude = menuItemGroupDefinition;
            _menuItemGroupDefinitionToExcludeParentText = menuItemGroupDefinition.Parent.Text;
        }
    }
}
