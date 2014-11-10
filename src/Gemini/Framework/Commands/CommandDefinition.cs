using System;
using System.Windows.Input;

namespace Gemini.Framework.Commands
{
    public abstract class CommandDefinition
    {
        public abstract string Name { get; }
        public abstract string Text { get; }
        public abstract string ToolTip { get; }

        public virtual Uri IconSource
        {
            get { return null; }
        }

        public virtual KeyGesture KeyGesture
        {
            get { return null; }
        }

        public virtual bool IsList
        {
            get { return false; }
        }
    }

    public abstract class CommandListDefinition : CommandDefinition
    {
        public override string Text
        {
            get { return "[NotUsed]"; }
        }

        public override string ToolTip
        {
            get { return "[NotUsed]"; }
        }

        public override bool IsList
        {
            get { return true; }
        }
    }
}