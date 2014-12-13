using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Demo.Xna.Modules.PrimitiveList.ViewModels;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Gemini.Demo.Xna.Modules.PrimitiveList.Commands
{
    [CommandHandler]
    public class ViewPrimitiveListCommandHandler : CommandHandlerBase<ViewPrimitiveListCommandDefinition>
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public ViewPrimitiveListCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.ShowTool<PrimitiveListViewModel>();
            return TaskUtility.Completed;
        }
    }
}