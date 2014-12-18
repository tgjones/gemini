using Gemini.Framework.Services;
using Gemini.Modules.Shell.Views;

namespace Gemini.Modules.Shell.Services
{
    public interface ILayoutItemStatePersister
    {
        void SaveState(IShell shell, IShellView shellView, string fileName);
        void LoadState(IShell shell, IShellView shellView, string fileName);
    }
}