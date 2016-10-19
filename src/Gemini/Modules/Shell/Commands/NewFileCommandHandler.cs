using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;
using Gemini.Properties;

namespace Gemini.Modules.Shell.Commands
{
    [CommandHandler]
    public class NewFileCommandHandler : ICommandListHandler<NewFileCommandListDefinition>
    {
        private readonly IEditorProvider[] _editorProviders;

        private readonly IShell _shell;
        private int _newItemCounter = 1;

        [ImportingConstructor]
        public NewFileCommandHandler(IShell shell, [ImportMany] IEditorProvider[] editorProviders)
        {
            _shell = shell;
            _editorProviders = editorProviders;
        }

        public void Populate(Command command, List<Command> commands)
        {
            foreach (var editorProvider in _editorProviders)
                foreach (var editorFileType in editorProvider.ItemTypes)
                    commands.Add(new Command(command.CommandDefinition)
                    {
                        Text = editorFileType.Name,
                        Tag = new NewItemTag
                        {
                            EditorProvider = editorProvider,
                            ItemType = editorFileType
                        }
                    });
        }

        public Task Run(Command command)
        {
            var tag = (NewItemTag) command.Tag;
            var editor = tag.EditorProvider.Create();
            var viewAware = (IViewAware) editor;
            viewAware.ViewAttached += (sender, e) =>
            {
                var frameworkElement = (FrameworkElement) e.View;
                RoutedEventHandler loadedHandler = null;
                loadedHandler = async (sender2, e2) =>
                {
                    frameworkElement.Loaded -= loadedHandler;
                    var itemName = tag.ItemType is EditorFileType
                        ? _newItemCounter++.ToString() + ((EditorFileType) tag.ItemType).FileExtension
                        : _newItemCounter++.ToString();
                    await tag.EditorProvider.New(editor, string.Format(Resources.FileNewUntitled, itemName));
                };
                frameworkElement.Loaded += loadedHandler;
            };
            _shell.OpenDocument(editor);
            return TaskUtility.Completed;
        }

        private class NewItemTag
        {
            public IEditorProvider EditorProvider;
            public EditorItemType ItemType;
        }
    }
}