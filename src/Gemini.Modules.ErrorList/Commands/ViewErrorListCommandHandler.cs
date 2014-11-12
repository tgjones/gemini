using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Gemini.Modules.ErrorList.Commands
{
    [CommandHandler(typeof(ViewErrorListCommandDefinition))]
    public class ViewErrorListCommandHandler : CommandHandler
    {
        [Import]
        private IShell _shell;

        public override Task Run(Command command)
        {
            _shell.ShowTool<IErrorList>();
            return TaskUtility.Completed;
        }
    }
}