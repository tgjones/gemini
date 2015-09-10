namespace Gemini.Framework.ToolBars
{
    public class ExcludeToolBarItemDefinition
    {
        private readonly ToolBarItemDefinition _ToolBarItemDefinitionToExclude;
        public ToolBarItemDefinition ToolBarItemDefinitionToExclude
        {
            get { return _ToolBarItemDefinitionToExclude; }
        }

        public ExcludeToolBarItemDefinition(ToolBarItemDefinition ToolBarItemDefinition)
        {
            _ToolBarItemDefinitionToExclude = ToolBarItemDefinition;
        }
    }
}