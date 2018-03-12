using Caliburn.Micro;
using Gemini.Demo.Modules.WebPage.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemini.Demo.Modules.WebPage.Commands
{
    [CommandHandler]
    public class ViewWebPageCommandHandler : CommandHandlerBase<ViewWebPageCommandDefinition>
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public ViewWebPageCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.OpenDocument((IDocument)IoC.GetInstance(typeof(WebPageViewModel), null));
            return TaskUtility.Completed;
        }
    }
}
