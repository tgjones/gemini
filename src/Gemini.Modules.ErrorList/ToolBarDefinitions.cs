using System.ComponentModel.Composition;
using Gemini.Framework.ToolBars;
using Gemini.Modules.ErrorList.Commands;

namespace Gemini.Modules.ErrorList
{
    public static class ToolBarDefinitions
    {
        public static ToolBarDefinition ErrorListToolBar = new ToolBarDefinition(0, "ErrorList");

        [Export]
        public static ToolBarItemGroupDefinition ToggleErrorsGroup = new ToolBarItemGroupDefinition(
            ErrorListToolBar, 0);

        [Export]
        public static ToolBarItemGroupDefinition ToggleWarningsGroup = new ToolBarItemGroupDefinition(
            ErrorListToolBar, 1);

        [Export]
        public static ToolBarItemGroupDefinition ToggleMessagesGroup = new ToolBarItemGroupDefinition(
            ErrorListToolBar, 2);

        [Export]
        public static ToolBarItemDefinition ToggleErrorsToolBarItem = new CommandToolBarItemDefinition<ToggleErrorsCommandDefinition>(
            ToggleErrorsGroup, 0, ToolBarItemDisplay.IconAndText);

        [Export]
        public static ToolBarItemDefinition ToggleWarningsToolBarItem = new CommandToolBarItemDefinition<ToggleWarningsCommandDefinition>(
            ToggleWarningsGroup, 1, ToolBarItemDisplay.IconAndText);

        [Export]
        public static ToolBarItemDefinition ToggleMessagesToolBarItem = new CommandToolBarItemDefinition<ToggleMessagesCommandDefinition>(
            ToggleMessagesGroup, 2, ToolBarItemDisplay.IconAndText);
    }
}