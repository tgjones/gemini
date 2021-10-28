using Gemini.Framework.ToolBars;
using Gemini.Modules.CodeEditor.Commands;
using System.ComponentModel.Composition;

namespace Gemini.Modules.CodeEditor
{
    public static class CodeEditorToolBarDefinitions
    {
        [Export]
        public static ToolBarDefinition CodeEditorToolBar = new ToolBarDefinition(0, "CodeEditorToolBar");

        [Export]
        public static ToolBarItemGroupDefinition CodeEditorToolBarGroup = new ToolBarItemGroupDefinition(
            CodeEditorToolBar, 10);

        [Export]
        public static ToolBarItemDefinition ShowLineNumbersToolBarItem = new CommandToolBarItemDefinition<ShowLineNumbersCommandDefinition>(
            CodeEditorToolBarGroup, 10, ToolBarItemDisplay.IconOnly);

        [Export]
        public static ToolBarItemDefinition ShowSpacesToolBarItem = new CommandToolBarItemDefinition<ShowSpacesCommandDefinition>(
            CodeEditorToolBarGroup, 20, ToolBarItemDisplay.IconOnly);

        [Export]
        public static ToolBarItemDefinition ShowTabsToolBarItem = new CommandToolBarItemDefinition<ShowTabsCommandDefinition>(
            CodeEditorToolBarGroup, 30, ToolBarItemDisplay.IconOnly);

        [Export]
        public static ToolBarItemDefinition ShowEndOfLineToolBarItem = new CommandToolBarItemDefinition<ShowEndOfLineCommandDefinition>(
            CodeEditorToolBarGroup, 40, ToolBarItemDisplay.IconOnly);

        [Export]
        public static ToolBarItemDefinition WordWrapToolBarItem = new CommandToolBarItemDefinition<WordWrapCommandDefinition>(
            CodeEditorToolBarGroup, 50, ToolBarItemDisplay.IconOnly);
    }
}
