using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Gemini.Framework;
using Gemini.Modules.ErrorList;
using Gemini.Modules.Inspector;
using Gemini.Modules.Output;
using Gemini.Modules.Toolbox;
using Gemini.Modules.UndoRedo;

namespace Gemini.Demo.Metro.Modules.Startup
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(IErrorList);
                yield return typeof(IHistoryTool);
                yield return typeof(IInspectorTool);
                yield return typeof(IOutput);
                yield return typeof(IToolbox);
            }
        }

        public override void Initialize()
        {
            MainWindow.Title = "Gemini Metro Demo";
        }
    }
}