using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Demo.Modules.Home.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Gemini.Demo.Modules.Home.Commands
{
    [CommandHandler]
    public class ViewHelixCommandHandler : CommandHandlerBase<ViewHelixCommandDefinition>
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public ViewHelixCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.OpenDocument((IDocument) IoC.GetInstance(typeof(HelixViewModel), null));
            return TaskUtility.Completed;
        }
    }
}