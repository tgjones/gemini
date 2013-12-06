using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Gemini.Framework;
using Gemini.Modules.Inspector;
using Gemini.Modules.Toolbox;

namespace Gemini.Demo.FilterDesigner.Modules.Startup
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(IInspectorTool);
                yield return typeof(IToolbox);
            }
        }

		public override void Initialize()
		{
			MainWindow.Title = "Gemini Filter Designer Demo";
		}
	}
}