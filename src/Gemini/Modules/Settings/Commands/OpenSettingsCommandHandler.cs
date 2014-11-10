using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework.Commands;
using Gemini.Framework.Threading;
using Gemini.Modules.Settings.ViewModels;

namespace Gemini.Modules.Settings.Commands
{
    [CommandHandler(typeof(OpenSettingsCommandDefinition))]
    public class OpenSettingsCommandHandler : CommandHandler
    {
        [Import]
        private IWindowManager _windowManager;

        public override Task Run(Command command)
        {
            _windowManager.ShowDialog(IoC.Get<SettingsViewModel>());
            return TaskUtility.Completed;
        }
    }
}