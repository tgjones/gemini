using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

namespace Gemini.Modules.PropertyGrid.Commands
{
    [CommandHandler]
    public class ViewPropertyGridCommandHandler : CommandHandlerBase<ViewPropertyGridCommandDefinition>
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public ViewPropertyGridCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            return _shell.ShowToolAsync<IPropertyGrid>();
        }
    }
}