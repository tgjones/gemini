using Gemini.Framework.Commands;
using Gemini.Modules.CodeEditor.Properties;
using System;

namespace Gemini.Modules.CodeEditor.Commands
{
    [CommandDefinition]
    public class WordWrapCommandDefinition : CommandDefinition
    {
        public const string CommandName = "CodeEditor.WordWrap";

        public override string Name => CommandName;

        public override string Text => Resources.WordWrapCommandText;

        public override string ToolTip => Resources.WordWrapCommandToolTip;

        public override Uri IconSource => new Uri("pack://application:,,,/Gemini.Modules.CodeEditor;component/Resources/Icons/WordWrap.png");
    }
}
