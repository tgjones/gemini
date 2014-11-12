using System.Collections.Generic;
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
        public override IEnumerable<IDocument> DefaultDocuments
        {
            get { yield return new GraphViewModel(IoC.Get<IInspectorTool>()); }
        }
    }
}