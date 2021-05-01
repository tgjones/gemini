using System;
using System.ComponentModel;
using System.Globalization;
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
            NotifyOfPropertyChange(() => Text);
            NotifyOfPropertyChange(() => IconSource);
            NotifyOfPropertyChange(() => ToolTip);
            NotifyOfPropertyChange(() => HasToolTip);
            NotifyOfPropertyChange(() => IsChecked);
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
        /// Remove mnemonics underscores used by menu from text.
        /// Also replace escaped/double underscores by a single underscore.
        /// </summary>
        private static string TrimMnemonics(string text)
        {
            var resultArray = new char[text.Length];

            int resultLength = 0;
            bool previousWasUnderscore = false;
            for (int textIndex = 0; textIndex < text.Length; textIndex++)
            {
                char c = text[textIndex];
                if (c == '_' && !previousWasUnderscore)
                {
                    previousWasUnderscore = true;
                    continue;
                }

                previousWasUnderscore = false;
                resultArray[resultLength++] = c;
            }

            return new string(resultArray, 0, resultLength);
        }
    }
}