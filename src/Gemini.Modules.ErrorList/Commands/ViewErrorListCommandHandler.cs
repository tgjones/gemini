using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

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
            return _shell.ShowToolAsync<IErrorList>();
        }
    }
}