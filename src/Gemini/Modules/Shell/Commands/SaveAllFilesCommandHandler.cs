using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;
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

        public override Task Run(Command command)
        {
            var tasks = new List<Task<Tuple<IPersistedDocument, bool>>>();
            
            foreach (var document in _shell.Documents)
            {
                var persistedDocument = document as IPersistedDocument;
                if (persistedDocument == null) continue;

                // skip if the file is new
                if (!persistedDocument.IsNew)
                {
                    tasks.Add(DoSaveAsync(persistedDocument));
                }
            }

            // TODO: display "Item(s) saved" in statusbar

            return TaskUtility.Completed;            
        }

        // http://stackoverflow.com/questions/19431494/how-to-use-await-in-a-loop
        private async Task<Tuple<IPersistedDocument, bool>> DoSaveAsync(IPersistedDocument persistedDocument)
        {
            var filePath = persistedDocument.FilePath;
            await persistedDocument.Save(filePath);

            return Tuple.Create(persistedDocument, true);
        }
    }
}
