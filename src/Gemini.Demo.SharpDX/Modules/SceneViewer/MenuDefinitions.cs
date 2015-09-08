using System.ComponentModel.Composition;
using Gemini.Demo.SharpDX.Modules.SceneViewer.Commands;
using Gemini.Framework.Menus;

namespace Gemini.Demo.SharpDX.Modules.SceneViewer
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition ViewSceneViewerMenuItem = new CommandMenuItemDefinition<ViewSceneViewerCommandDefinition>(
            Startup.Module.DemosMenuGroup, 1);
    }
}