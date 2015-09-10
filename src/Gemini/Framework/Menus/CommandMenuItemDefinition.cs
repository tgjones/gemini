using System;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework.Commands;

namespace Gemini.Framework.Menus
{
    public class CommandMenuItemDefinition<TCommandDefinition> : MenuItemDefinition
        where TCommandDefinition : CommandDefinitionBase
    {
        private readonly CommandDefinitionBase _commandDefinition;
        private readonly KeyGesture _keyGesture;

        public override string Text
        {
            get { return _commandDefinition.Text; }
        }

        public override Uri IconSource
        {
            get { return _commandDefinition.IconSource; }
        }

        public override KeyGesture KeyGesture
        {
            get { return _keyGesture; }
        }

        public override CommandDefinitionBase CommandDefinition
        {
            get { return _commandDefinition; }
        }

        public CommandMenuItemDefinition(MenuItemGroupDefinition group, int sortOrder)
            : base(group, sortOrder)
        {
            _commandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(TCommandDefinition));
            _keyGesture = IoC.Get<ICommandKeyGestureService>().GetPrimaryKeyGesture(_commandDefinition);
        }
    }
}