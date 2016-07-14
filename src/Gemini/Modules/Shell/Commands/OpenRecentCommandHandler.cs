using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;
using Gemini.Properties;
using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Gemini.Modules.Shell.Commands
{
    [CommandHandler]
    public class OpenRecentFileCommandHandler : ICommandListHandler<OpenRecentCommandListDefinition>
    {
        private int _newFileCounter = 1;

        private readonly IShell _shell;
        private readonly IEditorProvider[] _editorProviders;

        [ImportingConstructor]
        public OpenRecentFileCommandHandler(
            IShell shell,
            [ImportMany] IEditorProvider[] editorProviders)
        {
            _shell = shell;
            _editorProviders = editorProviders;
        }

        public void Populate(Command command, List<Command> commands)
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

        public Task Run(Command command)
        {
            var tag = (NewFileTag)command.Tag;
            var editor = tag.EditorProvider.Create();

            var viewAware = (IViewAware)editor;
            viewAware.ViewAttached += (sender, e) =>
            {
                var frameworkElement = (FrameworkElement)e.View;

                RoutedEventHandler loadedHandler = null;
                loadedHandler = async (sender2, e2) =>
                {
                    frameworkElement.Loaded -= loadedHandler;
                    await tag.EditorProvider.New(editor, string.Format(Resources.FileNewUntitled, (_newFileCounter++) + tag.FileType.FileExtension));
                };
                frameworkElement.Loaded += loadedHandler;
            };

            _shell.OpenDocument(editor);

            return TaskUtility.Completed;
        }

        private class NewFileTag
        {
            public IEditorProvider EditorProvider;
            public EditorFileType FileType;
        }
    }
}
