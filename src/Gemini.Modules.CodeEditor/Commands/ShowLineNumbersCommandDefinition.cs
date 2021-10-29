using Gemini.Framework.Commands;
using Gemini.Modules.CodeEditor.Properties;
using System;

namespace Gemini.Modules.CodeEditor.Commands
{
    [CommandDefinition]
    public class ShowLineNumbersCommandDefinition : CommandDefinition
    {
        public const string CommandName = "CodeEditor.ShowLineNumbers";

        public override string Name => CommandName;

        public override string Text => Resources.ShowLineNumbersCommandText;

        public override string ToolTip => Resources.ShowLineNumbersCommandToolTip;

        public override Uri IconSource => new Uri("pack://application:,,,/Gemini.Modules.CodeEditor;component/Resources/Icons/ShowLineNumbers.png");
    }
}
