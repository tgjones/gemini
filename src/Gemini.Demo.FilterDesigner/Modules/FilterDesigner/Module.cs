using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Demo.FilterDesigner.Modules.FilterDesigner.ViewModels;
using Gemini.Framework;
using Gemini.Modules.Inspector;

namespace Gemini.Demo.FilterDesigner.Modules.FilterDesigner
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override void PostInitialize()
        {
            Shell.OpenDocument(new GraphViewModel(IoC.Get<IInspectorTool>()));
        }
    }
}