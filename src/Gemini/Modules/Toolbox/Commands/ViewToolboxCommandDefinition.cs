using Gemini.Framework.Commands;

namespace Gemini.Modules.Toolbox.Commands
{
    [CommandDefinition]
    public class ViewToolboxCommandDefinition : CommandDefinition
    {
        public const string CommandName = "View.Toolbox";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "Toolbo_x"; }
        }

        public override string ToolTip
        {
            get { return "Toolbox"; }
        }
    }
}