using System.Collections.Generic;

namespace Gemini.Modules.Settings.ViewModels
{
    public class SettingsPageViewModel
    {
        public SettingsPageViewModel()
        {
            Children = new List<SettingsPageViewModel>();
            Editors = new List<ISettingsEditorBase>();
        }

        public string Name { get; set; }

        public List<ISettingsEditorBase> Editors { get; }

        public List<SettingsPageViewModel> Children { get; }
    }
}
