using Caliburn.Micro;

namespace Gemini.Framework.Services
{
	public interface IEditorProvider
	{
		bool Handles(string path);
		IScreen Create(string path);
	}
}