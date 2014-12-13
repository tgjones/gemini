using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

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
            _shell.ShowTool<IInspectorTool>();
            return TaskUtility.Completed;
        }
    }
}