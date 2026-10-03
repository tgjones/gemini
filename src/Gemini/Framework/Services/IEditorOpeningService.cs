using System;
using System.Threading.Tasks;

namespace Gemini.Framework.Services
{
    /// <summary>
    /// Presents editor documents and completes only after their provider initialization.
    /// </summary>
    public interface IEditorOpeningService
    {
        /// <summary>
        /// Reuses an open persisted document for the path or creates one with the first
        /// matching provider. Configuration runs before presentation.
        /// </summary>
        /// <exception cref="NotSupportedException">No provider handles <paramref name="path"/>.</exception>
        Task<IDocument> OpenFileAsync(string path, Action<IDocument> configure = null);

        /// <summary>
        /// Creates, presents, and initializes a new document with
        /// <paramref name="editorProvider"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">The provider cannot create documents.</exception>
        Task<IDocument> NewFileAsync(IEditorProvider editorProvider, string name);
    }
}
