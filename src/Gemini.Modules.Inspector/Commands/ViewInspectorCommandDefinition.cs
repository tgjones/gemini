using Gemini.Framework.Commands;
using Gemini.Modules.Inspector.Properties;

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
            get { return Resources.ViewInspectorCommandText; }
        }

        public override string ToolTip
        {
            get { return Resources.ViewInspectorCommandToolTip; }
        }
    }
}