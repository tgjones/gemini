using System.Collections.Generic;

namespace Gemini.Modules.Settings.ViewModels
{
    public class SettingsPageViewModel
    {
        public SettingsPageViewModel()
        {
            Children = new List<SettingsPageViewModel>();
        }

        public string Name { get; set; }
        public ISettingsEditor Editor { get; set; }
        public List<SettingsPageViewModel> Children { get; private set; }
    }
}