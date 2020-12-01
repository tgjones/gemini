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
			get { return _command.Text; }
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
    }
}