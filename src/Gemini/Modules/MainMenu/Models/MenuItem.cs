using System;
using System.Globalization;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework.Commands;
using Gemini.Framework.Menus;

namespace Gemini.Modules.MainMenu.Models
{
	public class MenuItem : MenuItemBase
	{
	    private readonly MenuDefinitionBase _menuDefinition;

		public string Text
		{
            get { return _menuDefinition.Text; }
		}

	    public bool HasIcon
	    {
	        get { return IconSource != null; }
	    }

        public Uri IconSource
	    {
            get { return _menuDefinition.IconSource; }
	    }

		public string InputGestureText
		{
			get
			{
                return _menuDefinition.KeyGesture == null
					? string.Empty
                    : _menuDefinition.KeyGesture.GetDisplayStringForCulture(CultureInfo.CurrentUICulture);
			}
		}

	    public ICommand Command
	    {
	        get
	        {
                if (_menuDefinition.CommandDefinition != null)
                    return new TargetableCommand(IoC.Get<ICommandService>().GetCommand(_menuDefinition.CommandDefinition));
	            return null;
	        }
	    }

        public bool IsChecked
        {
            get { return false; } // TODO
        }

        public MenuItem(MenuDefinitionBase menuDefinition)
        {
            _menuDefinition = menuDefinition;
        }
	}
}