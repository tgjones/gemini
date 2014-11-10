using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Gemini.Modules.UndoRedo.Commands
{
    [CommandHandler(typeof(ViewHistoryCommandDefinition))]
    public class ViewHistoryCommandHandler : CommandHandler
    {
        [Import]
        private IShell _shell;

        public override Task Run(Command command)
        {
            _shell.ShowTool<IHistoryTool>();
            return TaskUtility.Completed;
        }
    }
}