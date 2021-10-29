using Gemini.Framework.Commands;
using Gemini.Modules.CodeEditor.Properties;
using System;

namespace Gemini.Modules.CodeEditor.Commands
{
    [CommandDefinition]
    public class ShowSpacesCommandDefinition : CommandDefinition
    {
        public const string CommandName = "CodeEditor.ShowSpaces";

        public override string Name => CommandName;

        public override string Text => Resources.ShowSpacesCommandText;

        public override string ToolTip => Resources.ShowSpacesCommandToolTip;

        public override Uri IconSource => new Uri("pack://application:,,,/Gemini.Modules.CodeEditor;component/Resources/Icons/ShowSpaces.png");
    }
}
