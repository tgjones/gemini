using System.ComponentModel.Composition;
using Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner.ViewModels;
using Gemini.Framework;

namespace Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override void Initialize()
        {
            Shell.OpenDocument(new GraphViewModel());
        }
    }
}