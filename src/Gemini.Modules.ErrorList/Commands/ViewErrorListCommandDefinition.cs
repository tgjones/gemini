using Gemini.Framework.Commands;

namespace Gemini.Modules.ErrorList.Commands
{
    [CommandDefinition]
    public class ViewErrorListCommandDefinition : CommandDefinition
    {
        public const string CommandName = "View.ErrorList";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "Error L_ist"; }
        }

        public override string ToolTip
        {
            get { return "Error List"; }
        }
    }
}