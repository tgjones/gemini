using Gemini.Framework.Commands;

namespace Gemini.Modules.RecentFiles.Commands
{
    [CommandDefinition]
    public class OpenRecentFileCommandListDefinition : CommandListDefinition
    {
        public const string CommandName = "File.OpenRecentFileList";

        public override string Name
        {
            get { return CommandName; }
        }
    }
}