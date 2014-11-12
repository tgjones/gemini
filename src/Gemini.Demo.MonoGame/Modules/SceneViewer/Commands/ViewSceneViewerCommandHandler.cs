using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Demo.MonoGame.Modules.SceneViewer.ViewModels;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Gemini.Demo.MonoGame.Modules.SceneViewer.Commands
{
    [CommandHandler(typeof(ViewSceneViewerCommandDefinition))]
    public class ViewSceneViewerCommandHandler : CommandHandler
    {
        [Import]
        private IShell _shell;

        public override Task Run(Command command)
        {
            _shell.OpenDocument(new SceneViewModel());
            return TaskUtility.Completed;
        }
    }
}