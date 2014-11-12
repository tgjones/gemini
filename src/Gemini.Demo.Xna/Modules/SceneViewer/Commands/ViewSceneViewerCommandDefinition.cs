using System.ComponentModel.Composition;
using Gemini.Framework.Commands;

namespace Gemini.Demo.Xna.Modules.SceneViewer.Commands
{
    [Export(typeof(CommandDefinition))]
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