using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Gemini.Framework;
using Gemini.Modules.Inspector;

namespace Gemini.Demo.ShaderDesigner.Modules.Startup
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
        public override IEnumerable<Type> DefaultTools
        {
            get { yield return typeof(IInspectorTool); }
        }

		public override void Initialize()
		{
			Shell.Title = "Gemini Shader Designer Demo";
		}
	}
}