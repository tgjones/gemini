using System.ComponentModel.Composition;
using Gemini.Framework.Commands;

namespace Gemini.Modules.Inspector.Commands
{
    [Export(typeof(CommandDefinition))]
    public class ViewInspectorCommandDefinition : CommandDefinition
    {
        public const string CommandName = "View.Inspector";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "Inspector"; }
        }

        public override string ToolTip
        {
            get { return "Inspector"; }
        }
    }
}