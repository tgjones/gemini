using System.Threading.Tasks;

namespace Gemini.Modules.Settings
{
    public interface ISettingsEditorAsync : ISettingsEditorBase
    {
        Task ApplyChangesAsync();
    }
}
