using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Demo.Metro.Modules.Home.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Modules.MainMenu.Models;
using Gemini.Modules.PropertyGrid;

namespace Gemini.Demo.Metro.Modules.Home
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
		[Import]
		private IPropertyGrid _propertyGrid;

	    public override IEnumerable<IDocument> DefaultDocuments
	    {
	        get
	        {
                yield return IoC.Get<HomeViewModel>();
	        }
	    }

	    public override void Initialize()
		{
			MainMenu.All
				.First(x => x.Name == "View")
				.Add(new MenuItem("Home", OpenHome));
		}

        public override void PostInitialize()
        {
            _propertyGrid.SelectedObject = IoC.Get<HomeViewModel>();
            Shell.OpenDocument(IoC.Get<HomeViewModel>());
        }

		private IEnumerable<IResult> OpenHome()
		{
			yield return Show.Document<HomeViewModel>();
		}
	}
}