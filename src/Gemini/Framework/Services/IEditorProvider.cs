using System.Collections.Generic;

namespace Gemini.Framework.Services
{
    public interface IEditorProvider
	{
        IEnumerable<EditorFileType> FileTypes { get; }
		bool Handles(string path);
        IDocument CreateNew(string name);
		IDocument Open(string path);
	}
}