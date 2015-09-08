using Gemini.Framework.Commands;
using Gemini.Properties;

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
            get { return Resources.ToolsOptionsCommandText; }
        }

        public override string ToolTip
        {
            get { return Resources.ToolsOptionsCommandToolTip; }
        }
    }
}