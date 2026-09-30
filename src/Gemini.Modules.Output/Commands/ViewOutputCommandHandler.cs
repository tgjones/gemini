using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

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
            return _shell.ShowToolAsync<IOutput>();
        }
    }
}