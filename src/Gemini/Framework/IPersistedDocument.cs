using System.Threading.Tasks;

namespace Gemini.Framework
{
    public interface IPersistedDocument : IDocument
    {
        bool IsNew { get; }
        string DocumentName { get; }
        string DocumentPath { get; }
        DocumentType DocumentType { get; }

        Task New(string fileName);
        Task Load(string filePath);
        Task Save(string filePath);
    }
}