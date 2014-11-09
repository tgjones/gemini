using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Modules.MainMenu.Models;

namespace Gemini.Modules.Output
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
		public override void Initialize()
		{
            //var viewMenuItem = MainMenu.Find(KnownMenuItemNames.View);
            //if (viewMenuItem != null)
            //    viewMenuItem.Add(new MenuItem("Output", OpenOutput));
        }

        private IEnumerable<IResult> OpenOutput()
        {
            yield return Show.Tool<IOutput>();
        }
	}
}