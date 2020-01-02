using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework.Commands;
using Gemini.Framework.Threading;
using Gemini.Modules.Settings.ViewModels;

namespace Gemini.Modules.Settings.Commands
{
    [CommandHandler]
    public class OpenSettingsCommandHandler : CommandHandlerBase<OpenSettingsCommandDefinition>
    {
        private readonly IWindowManager _windowManager;

        [ImportingConstructor]
        public OpenSettingsCommandHandler(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        public override async Task Run(Command command)
        {
            await _windowManager.ShowDialogAsync(IoC.Get<SettingsViewModel>());
        }
    }
}
