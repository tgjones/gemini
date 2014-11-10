using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Demo.Metro.Modules.Home.Commands;
using Gemini.Demo.Metro.Modules.Home.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Gemini.Modules.PropertyGrid;

namespace Gemini.Demo.Metro.Modules.Home
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
        [Export]
        public static MenuItemGroupDefinition ViewDemoMenuGroup = new MenuItemGroupDefinition(
            Gemini.Modules.MainMenu.MenuDefinitions.ViewMenu, 10);

        [Export]
        public static MenuItemDefinition ViewHomeMenuItem = new CommandMenuItemDefinition<ViewHomeCommandDefinition>(
            ViewDemoMenuGroup, 0);

		[Import]
		private IPropertyGrid _propertyGrid;

	    public override IEnumerable<IDocument> DefaultDocuments
	    {
	        get { yield return IoC.Get<HomeViewModel>(); }
	    }

        public override void PostInitialize()
        {
            _propertyGrid.SelectedObject = IoC.Get<HomeViewModel>();
            Shell.OpenDocument(IoC.Get<HomeViewModel>());
        }
	}
}