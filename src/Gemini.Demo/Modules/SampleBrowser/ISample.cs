using System.Threading.Tasks;
using Gemini.Framework.Services;

namespace Gemini.Demo.Modules.SampleBrowser
{
    public interface ISample
    {
        string Name { get; }
        Task Activate(IShell shell);
    }
}
