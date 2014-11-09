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
    public class MainMenuViewModel : MenuModel, IPartImportsSatisfiedNotification
	{
		[Import] 
		private IShell _shell;

        [Import]
        private IMenuBuilder _menuBuilder;

	    [ImportMany]
        private IEditorProvider[] _editorProviders;

	    private bool _autoHide;

	    private readonly SettingsPropertyChangedEventManager<Properties.Settings> _settingsEventManager =
	        new SettingsPropertyChangedEventManager<Properties.Settings>(Properties.Settings.Default);

	    public MainMenuViewModel()
	    {
            //Add(
            //    new MenuItem(KnownMenuItemNames.File, Properties.Resources.MenuItemFile)
            //    {
            //        new MenuItem(KnownMenuItemNames.FileOpen, Properties.Resources.MenuItemOpen, OpenFile).WithIcon(),
            //        MenuItemBase.Separator,
            //        new MenuItem(KnownMenuItemNames.FileExit, Properties.Resources.MenuItemExit, Exit),
            //    },
            //    new MenuItem(KnownMenuItemNames.View, Properties.Resources.MenuItemView));

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

	    void IPartImportsSatisfiedNotification.OnImportsSatisfied()
	    {
            //var fileNewMenuItem = new MenuItem(KnownMenuItemNames.FileNew, Properties.Resources.MenuItemFileNew);
            //foreach (var editorProvider in _editorProviders)
            //    foreach (var editorFileType in editorProvider.FileTypes)
            //        fileNewMenuItem.Add(new MenuItem(
            //            editorFileType.FileExtension, editorFileType.Name,
            //            () => CreateNewFile(editorProvider, editorFileType)));

            //var fileMenuItem = Find(KnownMenuItemNames.File);
            //fileMenuItem.Children.Insert(0, fileNewMenuItem);

	        _menuBuilder.BuildMenuBar(MenuDefinitions.MainMenuBar, this);
	    }

	    private int _newFileCounter = 1;
	    private IEnumerable<IResult> CreateNewFile(IEditorProvider editorProvider, EditorFileType editorFileType)
	    {
	        var newDocument = editorProvider.CreateNew("Untitled " + (_newFileCounter++) + editorFileType.FileExtension);
	        yield return Show.Document(newDocument);
	    }
	}
}