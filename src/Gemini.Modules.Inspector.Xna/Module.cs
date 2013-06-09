using System.ComponentModel.Composition;
using Gemini.Framework;
using Gemini.Modules.Inspector.Conventions;
using Gemini.Modules.Inspector.Xna.Inspectors;
using Microsoft.Xna.Framework;

namespace Gemini.Modules.Inspector.Xna
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override void Initialize()
        {
            DefaultPropertyInspectors.InspectorBuilders.Add(new StandardPropertyEditorBuilder<Color, XnaColorEditorViewModel>());
            DefaultPropertyInspectors.InspectorBuilders.Add(new StandardPropertyEditorBuilder<Vector3, Vector3EditorViewModel>());
        }
    }
}