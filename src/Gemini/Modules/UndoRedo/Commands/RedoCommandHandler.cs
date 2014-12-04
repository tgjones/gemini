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
        private readonly IShell _shell;

        [ImportingConstructor]
        public RedoCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override void Update(Command command)
        {
            command.Enabled = (_shell.ActiveItem != null && _shell.ActiveItem.UndoRedoManager.RedoStack.Any());
        }

        public override Task Run(Command command)
        {
            _shell.ActiveItem.UndoRedoManager.Redo(1);
            return TaskUtility.Completed;
        }
    }
}