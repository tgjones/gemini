using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Microsoft.Win32;

namespace Gemini.Modules.Shell.Commands
{
    [CommandHandler]
    public class OpenFileCommandHandler : CommandHandlerBase<OpenFileCommandDefinition>
    {
        private readonly IEditorOpeningService _editorOpeningService;
        private readonly IEditorProvider[] _editorProviders;

        [ImportingConstructor]
        public OpenFileCommandHandler(
            IEditorOpeningService editorOpeningService,
            [ImportMany] IEditorProvider[] editorProviders)
        {
            _editorOpeningService = editorOpeningService;
            _editorProviders = editorProviders;
        }

        public override void Update(Command command)
        {
            base.Update(command);

            command.Enabled = _editorProviders != null && _editorProviders.Length > 0;
        }

        public override async Task Run(Command command)
        {
            var dialog = new OpenFileDialog();

            var filter = "All Supported Files|" + string.Join(";", _editorProviders
                .SelectMany(x => x.FileTypes).Select(x => "*" + x.FileExtension));

            filter += "|" + string.Join("|", _editorProviders
                .SelectMany(x => x.FileTypes)
                .Select(x => x.Name + "|*" + x.FileExtension));

            dialog.Filter = filter;

            if (dialog.ShowDialog() == true)
                await _editorOpeningService.OpenFileAsync(dialog.FileName);
        }
    }
}
