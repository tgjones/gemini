using System.ComponentModel.Composition;
using Gemini.Framework.Commands;

namespace Gemini.Demo.FilterDesigner.Modules.FilterDesigner.Commands
{
    [Export(typeof(CommandDefinition))]
    public class OpenGraphCommandDefinition : CommandDefinition
    {
        public const string CommandName = "File.OpenGraph";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "Open Graph"; }
        }

        public override string ToolTip
        {
            get { return "Open Graph"; }
        }
    }
}