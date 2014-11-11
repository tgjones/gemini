using System;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework.Commands;

namespace Gemini.Framework.ToolBars
{
    public class CommandToolBarItemDefinition<TCommandDefinition> : ToolBarItemDefinition
        where TCommandDefinition : CommandDefinition
    {
        private readonly CommandDefinition _commandDefinition;

        public override string Text
        {
            get { return _commandDefinition.ToolTip; }
        }

        public override Uri IconSource
        {
            get { return _commandDefinition.IconSource; }
        }

        public override KeyGesture KeyGesture
        {
            get { return _commandDefinition.KeyGesture; }
        }

        public override CommandDefinition CommandDefinition
        {
            get { return _commandDefinition; }
        }

        public CommandToolBarItemDefinition(ToolBarItemGroupDefinition group, int sortOrder)
            : base(group, sortOrder)
        {
            _commandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(TCommandDefinition));
        }
    }
}