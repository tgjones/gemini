using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework.Commands;
using Gemini.Framework.Menus;

namespace Gemini.Modules.MainMenu.Models
{
    public abstract class StandardMenuItem : MenuItemBase
    {
        public abstract string Text { get; }
        public abstract Uri IconSource { get; }
        public abstract string InputGestureText { get; }
        public abstract ICommand Command { get; }
        public abstract bool IsChecked { get; }
        public abstract bool IsVisible { get; }
    }

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
            get { return false; } // TODO
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
        }

        CommandDefinition ICommandUiItem.CommandDefinition
        {
            get { return _command.CommandDefinition; }
        }

	    void ICommandUiItem.Update(CommandHandler commandHandler)
	    {
	        if (_command != null && _command.CommandDefinition.IsList && !IsListItem)
	        {
	            foreach (var listItem in _listItems)
	                _parent.Children.Remove(listItem);

                _listItems.Clear();

	            var listCommands = new List<Command>();
                commandHandler.Update(listCommands);

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

	public class TextMenuItem : StandardMenuItem
	{
	    private readonly MenuDefinitionBase _menuDefinition;

        public override string Text
		{
            get { return _menuDefinition.Text; }
		}

        public override Uri IconSource
	    {
            get { return _menuDefinition.IconSource; }
	    }

        public override string InputGestureText
		{
			get
			{
                return _menuDefinition.KeyGesture == null
					? string.Empty
                    : _menuDefinition.KeyGesture.GetDisplayStringForCulture(CultureInfo.CurrentUICulture);
			}
		}

        public override ICommand Command
	    {
	        get { return null; }
	    }

        public override bool IsChecked
        {
            get { return false; }
        }

	    public override bool IsVisible
	    {
	        get { return true; }
	    }

	    public TextMenuItem(MenuDefinitionBase menuDefinition)
        {
            _menuDefinition = menuDefinition;
        }
	}
}