namespace Gemini.Framework.Menus
{
    [System.Diagnostics.DebuggerDisplay("Exclude {MenuItemGroupDefinitionToExclude}")]
    public class ExcludeMenuItemGroupDefinition
    {
        public MenuItemGroupDefinition MenuItemGroupDefinitionToExclude { get; private set; }

        public ExcludeMenuItemGroupDefinition(MenuItemGroupDefinition menuItemGroupDefinition)
        {
            MenuItemGroupDefinitionToExclude = menuItemGroupDefinition;
        }
    }
}
