using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Modules.Settings;

namespace Gemini.Modules.MainMenu.ViewModels
{
    [Export(typeof (ISettingsEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainMenuSettingsViewModel : PropertyChangedBase, ISettingsEditor
    {
        private bool _autoHideMainMenu;

        public MainMenuSettingsViewModel()
        {
            AutoHideMainMenu = Properties.Settings.Default.AutoHideMainMenu;
        }

        public bool AutoHideMainMenu
        {
            get { return _autoHideMainMenu; }
            set
            {
                if (value.Equals(_autoHideMainMenu)) return;
                _autoHideMainMenu = value;
                NotifyOfPropertyChange(() => AutoHideMainMenu);
            }
        }

        public string SettingsPageName
        {
            get { return Properties.Resources.SettingsPageGeneral; }
        }

        public string SettingsPagePath
        {
            get { return "Environment"; }
        }

        public void ApplyChanges()
        {
            if (AutoHideMainMenu == Properties.Settings.Default.AutoHideMainMenu)
            {
                return;
            }

            Properties.Settings.Default.AutoHideMainMenu = AutoHideMainMenu;
            Properties.Settings.Default.Save();
        }
    }
}