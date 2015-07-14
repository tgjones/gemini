using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Gemini.Modules.Shell.Commands
{
    [CommandHandler]
    public class CloseFileCommandHandler : CommandHandlerBase<CloseFileCommandDefinition>
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public CloseFileCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override void Update(Command command)
        {
            command.Enabled = _shell.ActiveItem != null;
            base.Update(command);
        }

        public override Task Run(Command command)
        {
            _shell.CloseDocument(_shell.ActiveItem);
            return TaskUtility.Completed;
        }
    }
}