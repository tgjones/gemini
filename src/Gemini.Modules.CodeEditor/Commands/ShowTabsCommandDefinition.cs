using Gemini.Framework.Commands;
using Gemini.Modules.CodeEditor.Properties;
using System;

namespace Gemini.Modules.CodeEditor.Commands
{
    [CommandDefinition]
    public class ShowTabsCommandDefinition : CommandDefinition
    {
        public const string CommandName = "CodeEditor.ShowTabs";

        public override string Name => CommandName;

        public override string Text => Resources.ShowTabsCommandText;

        public override string ToolTip => Resources.ShowTabsCommandToolTip;

        public override Uri IconSource => new Uri("pack://application:,,,/Gemini.Modules.CodeEditor;component/Resources/Icons/ShowTabs.png");
    }
}
