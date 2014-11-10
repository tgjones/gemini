using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Gemini.Modules.PropertyGrid.Commands
{
    [CommandHandler(typeof(ViewPropertyGridCommandDefinition))]
    public class ViewPropertyGridCommandHandler : CommandHandler
    {
        [Import]
        private IShell _shell;

        public override Task Run(Command command)
        {
            _shell.ShowTool<IPropertyGrid>();
            return TaskUtility.Completed;
        }
    }
}