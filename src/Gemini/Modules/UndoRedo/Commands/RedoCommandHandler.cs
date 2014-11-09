using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Gemini.Modules.UndoRedo.Commands
{
    [CommandHandler(typeof(RedoCommandDefinition))]
    public class RedoCommandHandler : CommandHandler
    {
        [Import]
        private IShell _shell;

        public override void Update(Command command)
        {
            command.Enabled = (_shell.ActiveItem != null && _shell.ActiveItem.UndoRedoManager.RedoStack.Any());
        }

        public override Task Run()
        {
            _shell.ActiveItem.UndoRedoManager.Redo(1);
            return TaskUtility.Completed;
        }
    }
}