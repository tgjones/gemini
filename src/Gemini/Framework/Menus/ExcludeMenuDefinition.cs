namespace Gemini.Framework.Menus
{
    [System.Diagnostics.DebuggerDisplay("Exclude {MenuDefinitionToExclude}")]
    public class ExcludeMenuDefinition
    {
        public MenuDefinition MenuDefinitionToExclude { get; private set; }

        public ExcludeMenuDefinition(MenuDefinition menuDefinition)
        {
            MenuDefinitionToExclude = menuDefinition;
        }
    }
}
