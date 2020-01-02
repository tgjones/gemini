using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Demo.Modules.FilterDesigner.ViewModels;
using Gemini.Demo.Modules.SampleBrowser;
using Gemini.Framework.Services;
using Gemini.Modules.Inspector;
using Gemini.Modules.Toolbox;

namespace Gemini.Demo.Modules.FilterDesigner
{
    [Export(typeof(ISample))]
    public class Sample : ISample
    {
        public string Name => "Filter Designer";

        public async Task Activate(IShell shell)
        {
            await shell.OpenDocumentAsync(IoC.Get<GraphViewModel>());
            shell.ShowTool<IInspectorTool>();
            shell.ShowTool<IToolbox>();
        }
    }
}
