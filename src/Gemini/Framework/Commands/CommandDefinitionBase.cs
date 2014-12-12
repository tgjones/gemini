using System;
using System.Windows.Input;

namespace Gemini.Framework.Commands
{
    public abstract class CommandDefinitionBase
    {
        public abstract string Name { get; }
        public abstract string Text { get; }
        public abstract string ToolTip { get; }
        public abstract Uri IconSource { get; }
        public abstract KeyGesture KeyGesture { get; }
        public abstract bool IsList { get; }
    }
}