using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Gemini.Modules.ErrorList.Commands
{
    [CommandHandler]
    public class ViewErrorListCommandHandler : CommandHandlerBase<ViewErrorListCommandDefinition>
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public ViewErrorListCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.ShowTool<IErrorList>();
            return TaskUtility.Completed;
        }
    }
}