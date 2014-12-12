using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework;
using Gemini.Framework.Commands; 
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Gemini.Modules.Shell.Commands
{
    [CommandHandler(typeof(SwitchToDocumentCommandListDefinition))]
    public class SwitchToDocumentCommandHandler : CommandHandler
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public SwitchToDocumentCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override void Update(Command command, List<Command> commands)
        {
            for (var i = 0; i < _shell.Documents.Count; i++)
            {
                var document = _shell.Documents[i];
                commands.Add(new Command(command.CommandDefinition)
                {
                    Checked = _shell.ActiveItem == document,
                    Text = string.Format("_{0} {1}", i + 1, document.DisplayName),
                    Tag = document
                });
            }
        }

        public override Task Run(Command command)
        {
            _shell.OpenDocument((IDocument) command.Tag);
            return TaskUtility.Completed;
        }
    }
}