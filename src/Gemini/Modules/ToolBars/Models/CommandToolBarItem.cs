using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework.Commands;
using Gemini.Framework.ToolBars;

namespace Gemini.Modules.ToolBars.Models
{
	public class CommandToolBarItem : ToolBarItemBase, ICommandUiItem
    {
	    private readonly ToolBarItemDefinition _toolBarItem;
	    private readonly Command _command;
        private readonly KeyGesture _keyGesture;
        private readonly IToolBar _parent;

		public string Text
		{
			get { return TrimMnemonics(_command.Text); }
		}

        public ToolBarItemDisplay Display
        {
            get { return _toolBarItem.Display; }
        }

	    public Uri IconSource
	    {
	        get { return _command.IconSource; }
	    }

	    public string ToolTip
	    {
	        get
	        {
                var inputGestureText = (_keyGesture != null)
                    ? string.Format(" ({0})", _keyGesture.GetDisplayStringForCulture(CultureInfo.CurrentUICulture))
                    : string.Empty;

                return string.Format("{0}{1}", _command.ToolTip, inputGestureText).Trim();
	        }
	    }

	    public bool HasToolTip
	    {
            get { return !string.IsNullOrWhiteSpace(ToolTip); }
	    }

        public ICommand Command
        {
            get { return IoC.Get<ICommandService>().GetTargetableCommand(_command); }
        }

        public bool IsChecked
        {
            get { return _command.Checked; }
        }

        public Visibility Visibility
        {
            get { return _command.Visible ? Visibility.Visible : Visibility.Collapsed; }
        }

		public CommandToolBarItem(ToolBarItemDefinition toolBarItem, Command command, IToolBar parent)
		{
		    _toolBarItem = toolBarItem;
		    _command = command;
            _keyGesture = IoC.Get<ICommandKeyGestureService>().GetPrimaryKeyGesture(_command.CommandDefinition);
            _parent = parent;

            command.PropertyChanged += OnCommandPropertyChanged;
		}

        private void OnCommandPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Framework.Commands.Command.Text):
                    NotifyOfPropertyChange(nameof(Text));
                    break;
                case nameof(Framework.Commands.Command.IconSource):
                    NotifyOfPropertyChange(nameof(IconSource));
                    break;
                case nameof(Framework.Commands.Command.ToolTip):
                    NotifyOfPropertyChange(nameof(ToolTip));
                    NotifyOfPropertyChange(nameof(HasToolTip));
                    break;
                case nameof(Framework.Commands.Command.Checked):
                    NotifyOfPropertyChange(nameof(IsChecked));
                    break;
                case nameof(Framework.Commands.Command.Visible):
                    NotifyOfPropertyChange(nameof(Visibility));
                    break;
            }
        }

	    CommandDefinitionBase ICommandUiItem.CommandDefinition
	    {
	        get { return _command.CommandDefinition; }
	    }

        void ICommandUiItem.Update(CommandHandlerWrapper commandHandler)
	    {
	        // TODO
	    }

        /// <summary>
        /// Remove mnemonics underscore used by menu from text.
        /// Also replace escaped/double underscores by a single underscore.
        /// Displayed text will be the same than with a menu item.
        /// </summary>
        private static string TrimMnemonics(string text)
        {
            var resultArray = new char[text.Length];

            int resultLength = 0;
            bool previousWasUnderscore = false;
            bool mnemonicsFound = false;

            for (int textIndex = 0; textIndex < text.Length; textIndex++)
            {
                char c = text[textIndex];

                if (c == '_')
                {
                    if (!previousWasUnderscore)
                    {
                        // If previous character was not an underscore but the current is one, we set the flag.
                        previousWasUnderscore = true;

                        // Also, if mnemonics mark was not found yet, we also skip that underscore in result.
                        if (!mnemonicsFound)
                            continue;
                    }
                    else
                    {
                        // If both current and previous character are underscores, it is an escaped underscore.
                        // We will include that second underscore in result and restore the flag.
                        previousWasUnderscore = false;

                        // If mnemonics mark was already found, previous underscore was included in result so we can escape this one.
                        if (mnemonicsFound)
                            continue;
                    }
                }
                else
                {
                    // If previous character was an underscore and the current is not one, we found the mnemonics mark.
                    // We will stop to search and include all the following characters, except escaped underscores, in result.
                    if (!mnemonicsFound && previousWasUnderscore)
                        mnemonicsFound = true;

                    previousWasUnderscore = false;
                }

                resultArray[resultLength++] = c;
            }

            // If last character was an underscore and mnemonics mark was not found, it should be included in result.
            if (previousWasUnderscore && !mnemonicsFound)
                resultArray[resultLength++] = '_';

            return new string(resultArray, 0, resultLength);
        }
    }
}