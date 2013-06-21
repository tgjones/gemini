using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Demo.FilterDesigner.Modules.ShaderDesigner.ViewModels;
using Gemini.Framework;
using Gemini.Modules.Inspector;

namespace Gemini.Demo.FilterDesigner.Modules.ShaderDesigner
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