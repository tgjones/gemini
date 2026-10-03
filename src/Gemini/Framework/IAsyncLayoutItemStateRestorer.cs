using System.Threading.Tasks;

namespace Gemini.Framework
{
    /// <summary>
    /// Restores asynchronous item content after synchronous state and layout parsing.
    /// The operation is awaited before restored items are activated and the shell is ready.
    /// </summary>
    public interface IAsyncLayoutItemStateRestorer
    {
        /// <summary>
        /// Restores content without using the binary reader from the completed parsing phase.
        /// </summary>
        Task RestoreStateAsync();
    }
}
