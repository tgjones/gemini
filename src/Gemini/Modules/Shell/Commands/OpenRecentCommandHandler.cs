﻿using Caliburn.Micro;
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
        private readonly IShell _shell;

        [ImportingConstructor]
        public OpenRecentFileCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public void Populate(Command command, List<Command> commands)
        {
            for (var i = 0; i < _shell.RecentFiles.Items.Count; i++)
            {
                var item = _shell.RecentFiles.Items[i];
                commands.Add(new Command(command.CommandDefinition)
                    {
                        Text = string.Format("_{0} {1}", i + 1, item.DisplayName),
                        ToolTip = item.FilePath,
                        Tag = item.FilePath
                    });
            }
        }

        public async Task Run(Command command)
        {
            var newPath = (string)command.Tag;

            // Check if the document is already open
            foreach (IDocument document in _shell.Documents)
            {
                if (document is PersistedDocument)
                {
                    var docPath = Path.GetFullPath(((PersistedDocument)document).FilePath);
                    if (string.Equals(newPath, docPath, System.StringComparison.OrdinalIgnoreCase))
                    {
                        _shell.OpenDocument(document);
                        return;
                    }
                }
            }

            _shell.OpenDocument(await GetEditor(newPath));

            // Add the file to the recent documents list
            _shell.RecentFiles.Update(newPath);
        }

        internal static Task<IDocument> GetEditor(string path)
        {
            var provider = IoC.GetAllInstances(typeof(IEditorProvider))
                .Cast<IEditorProvider>()
                .FirstOrDefault(p => p.Handles(path));
            if (provider == null)
                return null;

            var editor = provider.Create();

            var viewAware = (IViewAware)editor;
            viewAware.ViewAttached += (sender, e) =>
            {
                var frameworkElement = (FrameworkElement)e.View;

                RoutedEventHandler loadedHandler = null;
                loadedHandler = async (sender2, e2) =>
                {
                    frameworkElement.Loaded -= loadedHandler;
                    await provider.Open(editor, path);
                };
                frameworkElement.Loaded += loadedHandler;
            };

            return Task.FromResult(editor);
        }
    }
}