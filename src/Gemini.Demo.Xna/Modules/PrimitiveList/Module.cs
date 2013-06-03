using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Demo.Xna.Modules.PrimitiveList.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Modules.MainMenu.Models;

namespace Gemini.Demo.Xna.Modules.PrimitiveList
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
		public override void Initialize()
		{
			MainMenu.All
				.First(x => x.Name == "View")
				.Add(new MenuItem("Primitive List", OpenModelList));

			Shell.ShowTool(IoC.Get<PrimitiveListViewModel>());
		}

        private IEnumerable<IResult> OpenModelList()
		{
            yield return Show.Tool<PrimitiveListViewModel>();
		}
	}
}