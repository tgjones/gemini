using Gemini.Framework.Commands;

namespace Gemini.Demo.MonoGame.Modules.SceneViewer.Commands
{
    [CommandDefinition]
    public class ViewSceneViewerCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Demos.SceneViewer";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_3D Scene"; }
        }

        public override string ToolTip
        {
            get { return "3D Scene"; }
        }
    }
}