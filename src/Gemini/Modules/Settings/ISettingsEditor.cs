namespace Gemini.Modules.Settings
{
    public interface ISettingsEditor : ISettingsEditorBase
    {
        void ApplyChanges();
    }
}
