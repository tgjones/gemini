using System;
using System.Globalization;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework.Commands;

namespace Gemini.Modules.ToolBars.Models
{
	public class CommandToolBarItem : ToolBarItemBase, ICommandUiItem
    {
	    private readonly Command _command;
        private readonly IToolBar _parent;

		public string Text
		{
			get { return _command.IconSource == null ? _command.ToolTip : null; }
		}

	    public Uri IconSource
	    {
	        get { return _command.IconSource; }
	    }

	    public string ToolTip
	    {
	        get
	        {
                var inputGestureText = (_command.KeyGesture != null)
                    ? string.Format(" ({0})", _command.KeyGesture.GetDisplayStringForCulture(CultureInfo.CurrentUICulture))
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

		public CommandToolBarItem(Command command, IToolBar parent)
		{
		    _command = command;
		    _parent = parent;
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