using System.ComponentModel.Composition;
using Gemini.Framework;
using Gemini.Modules.GraphEditor.ViewModels;

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