using System.ComponentModel.Composition;
using Gemini.Framework.Commands;

namespace Gemini.Modules.Shell.Commands
{
    [Export(typeof(CommandDefinition))]
    public class SwitchToDocumentCommandDefinition : CommandListDefinition
    {
        public const string CommandName = "Window.SwitchToDocument";

        public override string Name
        {
            get { return CommandName; }
        }
    }
}