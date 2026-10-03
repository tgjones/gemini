using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

namespace Gemini.Modules.Inspector.Commands
{
    [CommandHandler]
    public class ViewInspectorCommandHandler : CommandHandlerBase<ViewInspectorCommandDefinition>
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public ViewInspectorCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            return _shell.ShowToolAsync<IInspectorTool>();
        }
    }
}