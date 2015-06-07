﻿using System;
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
        private readonly StandardMenuItem _parent;
        private readonly List<StandardMenuItem> _listItems;

        public override string Text
        {
            get { return _command.Text; }
        }

        public override Uri IconSource
        {
            get { return _command.IconSource; }
        }

        public override string InputGestureText
        {
            get
            {
                return _command.KeyGesture == null
                    ? string.Empty
                    : _command.KeyGesture.GetDisplayStringForCulture(CultureInfo.CurrentUICulture);
            }
        }

        public override ICommand Command
        {
            get { return IoC.Get<ICommandService>().GetTargetableCommand(_command); }
        }

        public override bool IsChecked
        {
            get { return _command.Checked; }
        }

        public override bool IsVisible
        {
            get { return _command.Visible; }
        }

        private bool IsListItem { get; set; }

        public CommandMenuItem(Command command, StandardMenuItem parent)
        {
            _command = command;
            _parent = parent;

            _listItems = new List<StandardMenuItem>();

            _command.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Visible" || e.PropertyName == "Checked")
                {
                    NotifyOfPropertyChange("Is" + e.PropertyName);
                }
                else if (e.PropertyName == "Text" || e.PropertyName == "IconSource")
                {
                    NotifyOfPropertyChange(e.PropertyName);
                }
            };
        }

        CommandDefinitionBase ICommandUiItem.CommandDefinition
        {
            get { return _command.CommandDefinition; }
        }

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
    }
}