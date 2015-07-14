using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gemini.Framework.Services
{
    public interface IEditorProvider
	{
        IEnumerable<EditorFileType> FileTypes { get; }
		bool Handles(string path);
        Task<IDocument> CreateNew(string name);
		Task<IDocument> Open(string path);
	}
}