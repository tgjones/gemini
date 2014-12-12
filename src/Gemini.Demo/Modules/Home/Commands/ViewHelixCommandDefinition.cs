using Gemini.Framework.Commands;

namespace Gemini.Demo.Modules.Home.Commands
{
    [CommandDefinition]
    public class ViewHelixCommandDefinition : CommandDefinition
    {
        public const string CommandName = "View.Helix";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "Helix"; }
        }

        public override string ToolTip
        {
            get { return "Helix"; }
        }
    }
}