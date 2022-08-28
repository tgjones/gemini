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

        public override string Text => _commandDefinition.Text;

        public override Uri IconSource => _commandDefinition.IconSource;

        public override KeyGesture KeyGesture => _keyGesture;

        public override CommandDefinitionBase CommandDefinition => _commandDefinition;

        public CommandMenuItemDefinition(MenuItemGroupDefinition group, int sortOrder)
            : base(group, sortOrder)
        {
            _commandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(TCommandDefinition));
            _keyGesture = IoC.Get<ICommandKeyGestureService>().GetPrimaryKeyGesture(_commandDefinition);
        }
    }
}
