using System.ComponentModel.Composition;
using Gemini.Framework.ToolBars;
using Gemini.Modules.Shell.Commands;

namespace Gemini.Modules.Shell
{
    public static class ToolBarDefinitions
    {
        [Export]
        public static ToolBarItemGroupDefinition StandardOpenSaveToolBarGroup = new ToolBarItemGroupDefinition(
            ToolBars.ToolBarDefinitions.StandardToolBar, 8);

        [Export]
        public static ToolBarItemDefinition OpenFileToolBarItem = new CommandToolBarItemDefinition<OpenFileCommandDefinition>(
            StandardOpenSaveToolBarGroup, 0);

        [Export]
        public static ToolBarItemDefinition SaveFileToolBarItem = new CommandToolBarItemDefinition<SaveFileCommandDefinition>(
            StandardOpenSaveToolBarGroup, 2);
    }
}