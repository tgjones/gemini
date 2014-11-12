using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Gemini.Modules.Shell.Commands
{
    [CommandHandler(typeof(NewFileCommandListDefinition))]
    public class NewFileCommandHandler : CommandHandler
    {
        private int _newFileCounter = 1;

        [Import]
        private ICommandService _commandService;

        [Import]
        private IShell _shell;

        [ImportMany]
        private IEditorProvider[] _editorProviders;

        public override void Update(Command command, List<Command> commands)
        {
            foreach (var editorProvider in _editorProviders)
                foreach (var editorFileType in editorProvider.FileTypes)
                    commands.Add(new Command(command.CommandDefinition)
                    {
                        Text = editorFileType.Name,
                        Tag = new NewFileTag
                        {
                            EditorProvider = editorProvider,
                            FileType = editorFileType
                        }
                    });
        }

        public override Task Run(Command command)
        {
            var tag = (NewFileTag) command.Tag;
            var newDocument = tag.EditorProvider.CreateNew("Untitled " + (_newFileCounter++) + tag.FileType.FileExtension);
            _shell.OpenDocument(newDocument);

            return TaskUtility.Completed;
        }

        private class NewFileTag
        {
            public IEditorProvider EditorProvider;
            public EditorFileType FileType;
        }
    }
}