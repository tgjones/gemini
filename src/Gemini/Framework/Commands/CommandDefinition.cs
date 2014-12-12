using System;
using System.Windows.Input;

namespace Gemini.Framework.Commands
{
    public abstract class CommandDefinition : CommandDefinitionBase
    {
        public override Uri IconSource
        {
            get { return null; }
        }

        public override KeyGesture KeyGesture
        {
            get { return null; }
        }

        public sealed override bool IsList
        {
            get { return false; }
        }
    }
}