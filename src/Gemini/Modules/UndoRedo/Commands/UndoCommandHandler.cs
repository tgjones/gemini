using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Gemini.Modules.UndoRedo.Commands
{
    [CommandHandler]
    public class UndoCommandHandler : CommandHandlerBase<UndoCommandDefinition>
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public UndoCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override void Update(Command command)
        {
            command.Enabled = (_shell.ActiveItem != null && _shell.ActiveItem.UndoRedoManager.UndoStack.Any());
        }

        public override Task Run(Command command)
        {
            _shell.ActiveItem.UndoRedoManager.Undo(1);
            return TaskUtility.Completed;
        }
    }
}