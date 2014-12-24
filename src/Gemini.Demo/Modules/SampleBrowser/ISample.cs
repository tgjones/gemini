using Gemini.Framework.Services;

namespace Gemini.Demo.Modules.SampleBrowser
{
    public interface ISample
    {
        string Name { get; }
        void Activate(IShell shell);
    }
}