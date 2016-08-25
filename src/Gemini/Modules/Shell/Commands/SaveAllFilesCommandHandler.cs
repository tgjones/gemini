using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Gemini.Modules.Shell.Commands
{
    [CommandHandler]
    public class SaveAllFilesCommandHandler : CommandHandlerBase<SaveAllFilesCommandDefinition>
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public SaveAllFilesCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override async Task Run(Command command)
        {
            var tasks = new List<Task<Tuple<IPersistedDocument, bool>>>();

            foreach (var document in _shell.Documents)
            {
                var persistedDocument = document as IPersistedDocument;
                if (persistedDocument == null) continue;

                // skip if the file is new
                if (!persistedDocument.IsNew)
                {
                    await persistedDocument.Save(persistedDocument.FilePath);
                }
            }

            // TODO: display "Item(s) saved" in statusbar
        }
    }
}