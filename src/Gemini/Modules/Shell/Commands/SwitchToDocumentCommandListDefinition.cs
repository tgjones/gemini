using Gemini.Framework.Commands;

namespace Gemini.Modules.Shell.Commands
{
    [CommandDefinition]
    public class SwitchToDocumentCommandListDefinition : CommandListDefinition
    {
        public const string CommandName = "Window.SwitchToDocument";

        public override string Name
        {
            get { return CommandName; }
        }
    }
}