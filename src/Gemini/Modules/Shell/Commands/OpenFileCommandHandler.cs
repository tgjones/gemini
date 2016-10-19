using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Microsoft.Win32;

namespace Gemini.Modules.Shell.Commands
{
    [CommandHandler]
    public class OpenFileCommandHandler : ICommandHandler<OpenFileCommandDefinition>
    {
        private readonly IShell _shell;
        private readonly IEditorProvider[] _editorProviders;

        [ImportingConstructor]
        public OpenFileCommandHandler(IShell shell, [ImportMany] IEditorProvider[] editorProviders)
        {
            _shell = shell;
            _editorProviders = editorProviders;
        }
        
        public void Update(Command command)
        {
            command.Enabled = _editorProviders.SelectMany(x => x.ItemTypes).OfType<EditorFileType>().Any();
        }

        public async Task Run(Command command)
        {
            var dialog = new OpenFileDialog();
            var itemTypes = _editorProviders.SelectMany(x => x.ItemTypes).OfType<EditorFileType>().ToList();
            dialog.Filter = Properties.Resources.AllSupportedFiles + "|" + string.Join(";", itemTypes.Select(x => "*" + x.FileExtension));
            dialog.Filter += "|" + string.Join("|", itemTypes.Select(x => x.Name + "|*" + x.FileExtension));
            if (dialog.ShowDialog() == true)
                _shell.OpenDocument(await OpenDocumentHelper.GetEditor(dialog.FileName));
        }
    }
}