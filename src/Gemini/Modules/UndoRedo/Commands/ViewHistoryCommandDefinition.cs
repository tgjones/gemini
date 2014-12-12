using Gemini.Framework.Commands;

namespace Gemini.Modules.UndoRedo.Commands
{
    [CommandDefinition]
    public class ViewHistoryCommandDefinition : CommandDefinition
    {
        public const string CommandName = "View.History";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_History"; }
        }

        public override string ToolTip
        {
            get { return "History"; }
        }
    }
}