using System;
using Gemini.Framework.Commands;

namespace Gemini.Modules.ErrorList.Commands
{
    [CommandDefinition]
    public class ToggleErrorsCommandDefinition : CommandDefinition
    {
        public const string CommandName = "ErrorList.ToggleErrors";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "[NotUsed]"; }
        }

        public override string ToolTip
        {
            get { return "[NotUsed]"; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Gemini.Modules.ErrorList;component/Resources/Error.png"); }
        }
    }
}