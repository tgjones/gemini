using System;
using System.Windows.Input;

namespace Gemini.Framework.Commands
{
    public abstract class CommandListDefinition : CommandDefinition
    {
        public override sealed string Text
        {
            get { return "[NotUsed]"; }
        }

        public override sealed string ToolTip
        {
            get { return "[NotUsed]"; }
        }

        public override sealed Uri IconSource
        {
            get { return null; }
        }

        public override sealed KeyGesture KeyGesture
        {
            get { return null; }
        }

        public override sealed bool IsList
        {
            get { return true; }
        }
    }
}