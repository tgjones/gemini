using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Demo.FilterDesigner.Modules.FilterDesigner.ViewModels;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;
using Gemini.Modules.Inspector;

namespace Gemini.Demo.FilterDesigner.Modules.FilterDesigner.Commands
{
    [CommandHandler(typeof(OpenGraphCommandDefinition))]
    public class OpenGraphCommandHandler : CommandHandler
    {
        [Import]
        private IShell _shell;

        public override Task Run(Command command)
        {
            _shell.OpenDocument(new GraphViewModel(IoC.Get<IInspectorTool>()));
            return TaskUtility.Completed;
        }
    }
}