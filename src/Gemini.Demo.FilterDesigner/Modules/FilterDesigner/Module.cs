using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Demo.FilterDesigner.Modules.FilterDesigner.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Modules.Inspector;
using Gemini.Modules.MainMenu.Models;

namespace Gemini.Demo.FilterDesigner.Modules.FilterDesigner
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override IEnumerable<IDocument> DefaultDocuments
        {
            get { yield return new GraphViewModel(IoC.Get<IInspectorTool>()); }
        }

        public override void Initialize()
        {
            MainMenu.All.First(x => x.Name == "File")
                .Children.Insert(1, new MenuItem("Open Graph", OpenGraph));
        }

        private static IEnumerable<IResult> OpenGraph()
        {
            yield return Show.Document(new GraphViewModel(IoC.Get<IInspectorTool>()));
        }
    }
}