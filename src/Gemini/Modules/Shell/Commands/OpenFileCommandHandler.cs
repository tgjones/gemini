using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;
using Microsoft.Win32;

namespace Gemini.Modules.Shell.Commands
{
    [CommandHandler]
    public class OpenFileCommandHandler : CommandHandlerBase<OpenFileCommandDefinition>
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public OpenFileCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override async Task Run(Command command)
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
                _shell.OpenDocument(await GetEditor(dialog.FileName));
        }

        private static async Task<IDocument> GetEditor(string path)
        {
            return await IoC.GetAllInstances(typeof(IEditorProvider))
                .Cast<IEditorProvider>()
                .Where(provider => provider.Handles(path))
                .Select(async provider => await provider.Open(path))
                .FirstOrDefault();
        }
    }
}