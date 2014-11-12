using System.ComponentModel.Composition;
using Gemini.Framework.Commands;

namespace Gemini.Modules.PropertyGrid.Commands
{
    [Export(typeof(CommandDefinition))]
    public class ViewPropertyGridCommandDefinition : CommandDefinition
    {
        public const string CommandName = "View.PropertiesWindow";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "Properties _Window"; }
        }

        public override string ToolTip
        {
            get { return "Properties Window"; }
        }
    }
}