using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Results;
using Gemini.Framework.Services;
using Gemini.Modules.MainMenu.Models;
using Microsoft.Win32;
using ExtensionMethods = Gemini.Framework.Services.ExtensionMethods;

namespace Gemini.Modules.MainMenu.ViewModels
{
	[Export(typeof(IMenu))]
	public class MainMenuViewModel : MenuModel
	{
		[Import] 
		private IShell _shell;

	    private bool _autoHide;

	    private readonly SettingsPropertyChangedEventManager<Properties.Settings> _settingsEventManager =
	        new SettingsPropertyChangedEventManager<Properties.Settings>(Properties.Settings.Default);

	    public MainMenuViewModel()
		{
			Add(
				new MenuItem(KnownMenuItemNames.File, Properties.Resources.MenuItemFile)
				{
					new MenuItem(KnownMenuItemNames.FileOpen, Properties.Resources.MenuItemOpen, OpenFile).WithIcon(),
					MenuItemBase.Separator,
					new MenuItem(KnownMenuItemNames.FileExit, Properties.Resources.MenuItemExit, Exit),
				},
                new MenuItem(KnownMenuItemNames.View, Properties.Resources.MenuItemView));

	        _autoHide = Properties.Settings.Default.AutoHideMainMenu;
            _settingsEventManager.AddListener(s => s.AutoHideMainMenu, value => { AutoHide = value; });
		}

	    public bool AutoHide
	    {
	        get { return _autoHide; }
	        private set
	        {
	            if (_autoHide == value)
	            {
	                return;
	            }

	            _autoHide = value;

	            NotifyOfPropertyChange(ExtensionMethods.GetPropertyName(() => AutoHide));
	        }
	    }

	    private IEnumerable<IResult> OpenFile()
		{
			var dialog = new OpenFileDialog();
			yield return Show.CommonDialog(dialog);
			yield return Show.Document(dialog.FileName);
		}

		private IEnumerable<IResult> Exit()
		{
			_shell.Close();
			yield break;
		}
	}
}