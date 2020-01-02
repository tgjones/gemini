using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Demo.Modules.SampleBrowser.ViewModels;
using Gemini.Framework;

namespace Gemini.Demo.Modules.SampleBrowser
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override Task PostInitializeAsync()
        {
            return Shell.OpenDocumentAsync(IoC.Get<SampleBrowserViewModel>());
        }
    }
}
