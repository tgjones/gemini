using Gemini.Framework.Commands;
using Gemini.Modules.ErrorList.Properties;

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
            get { return Resources.ViewErrorListCommandText; }
        }

        public override string ToolTip
        {
            get { return Resources.ViewErrorListCommandToolTip; }
        }
    }
}