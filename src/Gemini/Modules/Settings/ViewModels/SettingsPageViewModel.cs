using System.Collections.Generic;

namespace Gemini.Modules.Settings.ViewModels
{
    public class SettingsPageViewModel
    {
        public SettingsPageViewModel()
        {
            Children = new List<SettingsPageViewModel>();
            Editors = new List<object>();
        }

        public string Name { get; set; }

        public List<object> Editors { get; } // ISettingsEditor or ISettingsEditorAsync

        public List<SettingsPageViewModel> Children { get; }
    }
}
