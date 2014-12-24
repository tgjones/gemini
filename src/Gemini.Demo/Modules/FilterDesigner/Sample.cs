using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Demo.Modules.FilterDesigner.ViewModels;
using Gemini.Demo.Modules.SampleBrowser;
using Gemini.Framework.Services;
using Gemini.Modules.Inspector;

namespace Gemini.Demo.Modules.FilterDesigner
{
    [Export(typeof(ISample))]
    public class Sample : ISample
    {
        public string Name
        {
            get { return "Filter Designer"; }
        }

        public void Activate(IShell shell)
        {
            shell.OpenDocument(IoC.Get<GraphViewModel>());
            shell.ShowTool<IInspectorTool>();
        }
    }
}