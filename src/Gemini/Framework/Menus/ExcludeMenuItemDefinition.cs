namespace Gemini.Framework.Menus
{
    [System.Diagnostics.DebuggerDisplay("Exclude {MenuItemDefinitionToExclude}")]
    public class ExcludeMenuItemDefinition
    {
        public MenuItemDefinition MenuItemDefinitionToExclude { get; private set; }

        public ExcludeMenuItemDefinition(MenuItemDefinition menuItemDefinition)
        {
            MenuItemDefinitionToExclude = menuItemDefinition;
        }
    }
}
