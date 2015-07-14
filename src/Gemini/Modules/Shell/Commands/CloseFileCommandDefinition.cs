using Gemini.Framework.Commands;

namespace Gemini.Modules.Shell.Commands
{
    [CommandDefinition]
    public class CloseFileCommandDefinition : CommandDefinition
    {
        public const string CommandName = "File.CloseFile";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_Close"; }
        }

        public override string ToolTip
        {
            get { return "Close"; }
        }
    }
}