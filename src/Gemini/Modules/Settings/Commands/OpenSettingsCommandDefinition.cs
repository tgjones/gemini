using Gemini.Framework.Commands;

namespace Gemini.Modules.Settings.Commands
{
    [CommandDefinition]
    public class OpenSettingsCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Tools.Options";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_Options"; }
        }

        public override string ToolTip
        {
            get { return "Options"; }
        }
    }
}