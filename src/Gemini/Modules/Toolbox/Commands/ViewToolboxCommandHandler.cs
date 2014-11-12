using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Gemini.Modules.Toolbox.Commands
{
    [CommandHandler(typeof(ViewToolboxCommandDefinition))]
    public class ViewToolboxCommandHandler : CommandHandler
    {
        [Import]
        private IShell _shell;

        public override Task Run(Command command)
        {
            _shell.ShowTool<IToolbox>();
            return TaskUtility.Completed;
        }
    }
}