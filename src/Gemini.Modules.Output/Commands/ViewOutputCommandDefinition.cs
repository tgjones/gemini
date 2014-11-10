using System.ComponentModel.Composition;
using Gemini.Framework.Commands;

namespace Gemini.Modules.Output.Commands
{
    [Export(typeof(CommandDefinition))]
    public class ViewOutputCommandDefinition : CommandDefinition
    {
        public const string CommandName = "View.Output";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_Output"; }
        }

        public override string ToolTip
        {
            get { return "Output"; }
        }
    }
}