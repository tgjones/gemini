using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Gemini.Modules.Inspector.Commands
{
    [CommandHandler(typeof(ViewInspectorCommandDefinition))]
    public class ViewInspectorCommandHandler : CommandHandler
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