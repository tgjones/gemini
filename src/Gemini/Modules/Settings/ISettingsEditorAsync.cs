using System.Threading.Tasks;

namespace Gemini.Modules.Settings
{
    public interface ISettingsEditorAsync
    {
        string SettingsPageName { get; }
        string SettingsPagePath { get; }

        Task ApplyChangesAsync();
    }
}
