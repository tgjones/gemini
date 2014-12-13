using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Gemini.Modules.Output.Commands
{
    [CommandHandler]
    public class ViewOutputCommandHandler : CommandHandlerBase<ViewOutputCommandDefinition>
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public ViewOutputCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.ShowTool<IOutput>();
            return TaskUtility.Completed;
        }
    }
}