using System.Threading.Tasks;

namespace Gemini.Modules.Settings.ViewModels
{
    internal sealed class SettingsEditorWrapper : ISettingsEditorAsync
    {
        private readonly ISettingsEditor _editor;

        public SettingsEditorWrapper(ISettingsEditor editor)
        {
            _editor = editor;
        }

        public string SettingsPageName => _editor.SettingsPageName;

        public string SettingsPagePath => _editor.SettingsPagePath;

        public ISettingsEditor ViewModel => _editor;

        public Task ApplyChangesAsync()
        {
            _editor.ApplyChanges();
            return Task.CompletedTask;
        }
    }
}
