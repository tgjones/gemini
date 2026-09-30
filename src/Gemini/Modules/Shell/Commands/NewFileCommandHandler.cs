using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Properties;

namespace Gemini.Modules.Shell.Commands
{
    [CommandHandler]
    public class NewFileCommandHandler : ICommandListHandler<NewFileCommandListDefinition>
    {
        private int _newFileCounter = 1;

        private readonly IEditorOpeningService _editorOpeningService;
        private readonly IEditorProvider[] _editorProviders;

        [ImportingConstructor]
        public NewFileCommandHandler(
            IEditorOpeningService editorOpeningService,
            [ImportMany] IEditorProvider[] editorProviders)
        {
            _editorOpeningService = editorOpeningService;
            _editorProviders = editorProviders;
        }

        public void Populate(Command command, List<Command> commands)
        {
            foreach (var editorProvider in _editorProviders)
            {
                if (!editorProvider.CanCreateNew)
                    continue;

                foreach (var editorFileType in editorProvider.FileTypes)
                {
                    commands.Add(new Command(command.CommandDefinition)
                    {
                        Text = editorFileType.Name,
                        IconSource = editorFileType.IconSource,
                        Tag = new NewFileTag
                        {
                            EditorProvider = editorProvider,
                            FileType = editorFileType
                        }
                    });
                }
            }
        }

        public async Task Run(Command command)
        {
            var tag = (NewFileTag)command.Tag;
            var name = string.Format(
                Resources.FileNewUntitled,
                (_newFileCounter++) + tag.FileType.FileExtension);

            await _editorOpeningService.NewFileAsync(tag.EditorProvider, name);
        }

        private class NewFileTag
        {
            public IEditorProvider EditorProvider;
            public EditorFileType FileType;
        }
    }
}
