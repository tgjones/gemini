using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

namespace Gemini.Modules.Shell.Commands
{
    [CommandHandler]
    public class OpenFolderCommandHandler : ICommandHandler<OpenFolderCommandDefinition>
    {
        private readonly IShell _shell;
        private readonly IEditorProvider[] _editorProviders;

        [ImportingConstructor]
        public OpenFolderCommandHandler(IShell shell, [ImportMany] IEditorProvider[] editorProviders)
        {
            _shell = shell;
            _editorProviders = editorProviders;
        }

        public void Update(Command command)
        {
            command.Enabled = _editorProviders.SelectMany(x => x.ItemTypes).OfType<EditorFolderType>().Any();
        }

        public async Task Run(Command command)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    _shell.OpenDocument(await OpenDocumentHelper.GetEditor(dialog.SelectedPath));
            }
        }
    }
}