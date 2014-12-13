using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Demo.MonoGame.Modules.SceneViewer.ViewModels;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Gemini.Demo.MonoGame.Modules.SceneViewer.Commands
{
    [CommandHandler]
    public class ViewSceneViewerCommandHandler : CommandHandlerBase<ViewSceneViewerCommandDefinition>
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public ViewSceneViewerCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.OpenDocument(new SceneViewModel());
            return TaskUtility.Completed;
        }
    }
}