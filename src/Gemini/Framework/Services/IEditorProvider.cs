using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gemini.Framework.Services
{
    public interface IEditorProvider
	{
        IEnumerable<EditorItemType> ItemTypes { get; }
        bool Handles(string path);
        IDocument Create();
        Task New(IDocument document, string name);
        Task Open(IDocument document, string path);
	}
}