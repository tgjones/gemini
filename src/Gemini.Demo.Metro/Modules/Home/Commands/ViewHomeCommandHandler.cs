using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Demo.Metro.Modules.Home.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Gemini.Demo.Metro.Modules.Home.Commands
{
    [CommandHandler(typeof(ViewHomeCommandDefinition))]
    public class ViewHomeCommandHandler : CommandHandler
    {
        [Import]
        private IShell _shell;

        public override Task Run(Command command)
        {
            _shell.OpenDocument((IDocument) IoC.GetInstance(typeof(HomeViewModel), null));
            return TaskUtility.Completed;
        }
    }
}