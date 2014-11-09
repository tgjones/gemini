using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Demo.Modules.Home.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Modules.MainMenu.Models;
using Gemini.Modules.PropertyGrid;

namespace Gemini.Demo.Modules.Home
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
                yield return IoC.Get<HelixViewModel>();
	        }
	    }

	    public override void Initialize()
		{
            //MainMenu.Find(KnownMenuItemNames.View).Add(new MenuItem("Home", OpenHome));
            //MainMenu.Find(KnownMenuItemNames.View).Add(new MenuItem("Helix", OpenHelix));
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

        private IEnumerable<IResult> OpenHelix()
        {
            yield return Show.Document<HelixViewModel>();
        }
	}
}