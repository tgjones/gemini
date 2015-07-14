using Gemini.Framework.Commands;

namespace Gemini.Modules.Shell.Commands
{
    [CommandDefinition]
    public class SaveFileAsCommandDefinition : CommandDefinition
    {
        public const string CommandName = "File.SaveFileAs";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "Save _As..."; }
        }

        public override string ToolTip
        {
            get { return "Save As"; }
        }
    }
}