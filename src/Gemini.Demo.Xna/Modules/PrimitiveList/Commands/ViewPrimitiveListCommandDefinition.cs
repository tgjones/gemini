using Gemini.Framework.Commands;

namespace Gemini.Demo.Xna.Modules.PrimitiveList.Commands
{
    [CommandDefinition]
    public class ViewPrimitiveListCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Demos.PrimitiveList";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_Primitive List"; }
        }

        public override string ToolTip
        {
            get { return "Primitive List"; }
        }
    }
}