using System.ComponentModel.Composition;
using Gemini.Framework.Commands;

namespace Gemini.Modules.Shell.Commands
{
    [Export(typeof(CommandDefinition))]
    public class NewFileCommandListDefinition : CommandListDefinition
    {
        public const string CommandName = "File.NewFile";

        public override string Name
        {
            get { return CommandName; }
        }
    }
}