using System.Threading.Tasks;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.Views;

namespace Gemini.Modules.Shell.Services
{
    /// <summary>Persists the shell layout without changing its serialized format.</summary>
    public interface ILayoutItemStatePersister
    {
        /// <summary>
        /// Writes a complete candidate before replacing the destination and reports
        /// whether item payloads or the file operation failed.
        /// </summary>
        /// <remarks>
        /// Save remains synchronous because item and AvalonDock serialization write to
        /// one caller-owned stream synchronously. Only restoration has asynchronous
        /// content and lifecycle work after its stream-scoped parsing phase.
        /// </remarks>
        LayoutItemStateSaveResult SaveState(IShell shell, IShellView shellView, string fileName);

        /// <summary>
        /// Parses state synchronously while its stream is valid, then awaits optional
        /// item content restoration before returning the activation plan.
        /// </summary>
        Task<LayoutItemStateLoadResult> LoadStateAsync(
            IShell shell,
            IShellView shellView,
            string fileName);
    }
}