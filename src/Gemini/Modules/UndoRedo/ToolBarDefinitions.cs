using System.ComponentModel.Composition;
using Gemini.Framework.ToolBars;
using Gemini.Modules.UndoRedo.Commands;

namespace Gemini.Modules.UndoRedo
{
    public static class ToolBarDefinitions
    {
        [Export]
        public static ToolBarItemGroupDefinition StandardUndoRedoToolBarGroup = new ToolBarItemGroupDefinition(
            ToolBars.ToolBarDefinitions.StandardToolBar, 10);

        [Export]
        public static ToolBarItemDefinition UndoToolBarItem = new CommandToolBarItemDefinition<UndoCommandDefinition>(
            StandardUndoRedoToolBarGroup, 0);

        [Export]
        public static ToolBarItemDefinition RedoToolBarItem = new CommandToolBarItemDefinition<RedoCommandDefinition>(
            StandardUndoRedoToolBarGroup, 1);
    }
}