using Caliburn.PresentationFramework.ApplicationModel;

namespace Gemini.Framework.Services
{
	public interface IEditorProvider
    {
        bool Handles(string path);
        IExtendedPresenter Create(string path);
    }
}