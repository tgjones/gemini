using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework.Commands;

namespace Gemini.Modules.MainMenu.Models
{
    public class CommandMenuItem : StandardMenuItem, ICommandUiItem
    {
        private readonly Command _command;
        private readonly KeyGesture _keyGesture;
        private readonly StandardMenuItem _parent;
        private readonly List<StandardMenuItem> _listItems;

        public override string Text => _command.Text;

        public override Uri IconSource => _command.IconSource;

        public override string InputGestureText => _keyGesture == null
            ? string.Empty
            : _keyGesture.GetDisplayStringForCulture(CultureInfo.CurrentUICulture);

        public override ICommand Command => IoC.Get<ICommandService>().GetTargetableCommand(_command);

        public override bool IsChecked => _command.Checked;

        public override bool IsVisible => _command.Visible;

        private bool IsListItem { get; set; }

        public CommandMenuItem(Command command, StandardMenuItem parent)
        {
            _command = command;
            _keyGesture = IoC.Get<ICommandKeyGestureService>().GetPrimaryKeyGesture(_command.CommandDefinition);
            _parent = parent;

            _listItems = new List<StandardMenuItem>();

            _command.PropertyChanged += CommandPropertyChanged;
        }

        CommandDefinitionBase ICommandUiItem.CommandDefinition => _command.CommandDefinition;

        void ICommandUiItem.Update(CommandHandlerWrapper commandHandler)
        {
            if (_command != null && _command.CommandDefinition.IsList && !IsListItem)
            {
                foreach (var listItem in _listItems)
                    _parent.Children.Remove(listItem);

                _listItems.Clear();

                var listCommands = new List<Command>();
                commandHandler.Populate(_command, listCommands);

                _command.Visible = false;

                int startIndex = _parent.Children.IndexOf(this) + 1;

                foreach (var command in listCommands)
                {
                    var newMenuItem = new CommandMenuItem(command, _parent)
                    {
                        IsListItem = true
                    };
                    _parent.Children.Insert(startIndex++, newMenuItem);
                    _listItems.Add(newMenuItem);
                }
            }
        }

        private void CommandPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_command.Visible):
                    NotifyOfPropertyChange(nameof(IsVisible));
                    break;

                case nameof(_command.Checked):
                    NotifyOfPropertyChange(nameof(IsChecked));
                    break;

                case nameof(_command.Text):
                case nameof(_command.IconSource):
                    NotifyOfPropertyChange(e.PropertyName);
                    break;
            }
        }
    };
}
