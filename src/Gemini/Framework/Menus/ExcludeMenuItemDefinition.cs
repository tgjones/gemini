using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemini.Framework.Menus
{
    public class ExcludeMenuItemDefinition
    {
        private MenuItemDefinition _menuItemDefinitionToExclude;
        public MenuItemDefinition MenuItemDefinitionToExclude 
        { 
            get { return _menuItemDefinitionToExclude; } 
        }

        private string _menuItemDefinitionToExcludeText;
        public string MenuItemDefinitionToExcludeText
        {
            get { return _menuItemDefinitionToExcludeText; }
        }

        public ExcludeMenuItemDefinition(MenuItemDefinition menuItemDefinition)
        {
            _menuItemDefinitionToExclude = menuItemDefinition;
            _menuItemDefinitionToExcludeText = menuItemDefinition.Text;
        }
    }
}
