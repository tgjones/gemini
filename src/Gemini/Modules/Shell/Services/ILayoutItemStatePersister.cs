using System.Threading.Tasks;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.Views;

namespace Gemini.Modules.Shell.Services
{
    /// <summary>Persists the shell layout without changing its serialized format.</summary>
    public interface ILayoutItemStatePersister
    {
        bool SaveState(IShell shell, IShellView shellView, string fileName);

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