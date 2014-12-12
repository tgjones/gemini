using Gemini.Framework.Commands;

namespace Gemini.Demo.Modules.Home.Commands
{
    [CommandDefinition]
    public class ViewHomeCommandDefinition : CommandDefinition
    {
        public const string CommandName = "View.Home";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "Home"; }
        }

        public override string ToolTip
        {
            get { return "Home"; }
        }
    }
}