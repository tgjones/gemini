using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Gemini.Modules.Toolbox.Commands
{
    [CommandHandler]
    public class ViewToolboxCommandHandler : CommandHandlerBase<ViewToolboxCommandDefinition>
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public ViewToolboxCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.ShowTool<IToolbox>();
            return TaskUtility.Completed;
        }
    }
}