using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Gemini.Framework;
using Gemini.Modules.Toolbox;

namespace Gemini.Demo.Metro.Modules.Startup
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(IToolbox);
            }
        }

        public override void Initialize()
        {
            MainWindow.Title = "Gemini Metro Demo";
        }
    }
}