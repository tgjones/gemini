using Gemini.Framework.Commands;

namespace Gemini.Modules.PropertyGrid.Commands
{
    [CommandDefinition]
    public class ViewPropertyGridCommandDefinition : CommandDefinition
    {
        public const string CommandName = "View.PropertiesWindow";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "Properties _Window"; }
        }

        public override string ToolTip
        {
            get { return "Properties Window"; }
        }
    }
}