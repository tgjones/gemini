using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.Commands;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Gemini.Modules.RecentFiles.Commands
{
    [CommandHandler]
    public class OpenRecentFileCommandHandler : ICommandListHandler<OpenRecentFileCommandListDefinition>
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
            foreach (var document in _shell.Documents.OfType<PersistedDocument>().Where(d => !d.IsNew))
            {
                if (string.IsNullOrEmpty(document.FilePath))
                    continue;

                var docPath = Path.GetFullPath(document.FilePath);
                if (string.Equals(newPath, docPath, System.StringComparison.OrdinalIgnoreCase))
                {
                    _shell.OpenDocument(document);
                    return;
                }
            }

            _shell.OpenDocument(await OpenFileCommandHandler.GetEditor(newPath));

            // Add the file to the recent documents list
            _shell.RecentFiles.Update(newPath);
        }
    }
}