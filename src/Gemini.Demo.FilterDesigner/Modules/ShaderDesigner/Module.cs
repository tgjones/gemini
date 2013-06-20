using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner.ViewModels;
using Gemini.Framework;
using Gemini.Modules.Inspector;

namespace Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner
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