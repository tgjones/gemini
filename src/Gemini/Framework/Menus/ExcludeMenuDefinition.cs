using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemini.Framework.Menus
{
    public class ExcludeMenuDefinition
    {
        private MenuDefinition _menuDefinitionToExclude;
        public MenuDefinition MenuDefinitionToExclude 
        { 
            get { return _menuDefinitionToExclude; } 
        }

        private string _menuDefinitionToExcludeText;
        public string MenuDefinitionToExcludeText
        {
            get { return _menuDefinitionToExcludeText; }
        }

        public ExcludeMenuDefinition(MenuDefinition menuDefinition)
        {
            _menuDefinitionToExclude = menuDefinition;
            _menuDefinitionToExcludeText = menuDefinition.Text;
        }
    }
}
