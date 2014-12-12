using Gemini.Framework.Commands;

namespace Gemini.Modules.Inspector.Commands
{
    [CommandDefinition]
    public class ViewInspectorCommandDefinition : CommandDefinition
    {
        public const string CommandName = "View.Inspector";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "I_nspector"; }
        }

        public override string ToolTip
        {
            get { return "Inspector"; }
        }
    }
}