namespace Gemini.Framework.Services
{
	public interface IEditorProvider
	{
		bool Handles(string path);
		IDocument Create(string path);
	}
}