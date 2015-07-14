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
        private readonly IEditorProvider[] _editorProviders;

        [ImportingConstructor]
        public OpenFileCommandHandler(IShell shell, [ImportMany] IEditorProvider[] editorProviders)
        {
            _shell = shell;
            _editorProviders = editorProviders;
        }

        public override async Task Run(Command command)
        {
            var dialog = new OpenFileDialog();

            dialog.Filter = "All Supported Files|" + string.Join(";", _editorProviders
                .SelectMany(x => x.FileTypes).Select(x => "*" + x.FileExtension));

            dialog.Filter += "|" + string.Join("|", _editorProviders
                .SelectMany(x => x.FileTypes)
                .Select(x => x.Name + "|*" + x.FileExtension));

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