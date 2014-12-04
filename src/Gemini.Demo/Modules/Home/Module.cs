using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Demo.Modules.Home.Commands;
using Gemini.Demo.Modules.Home.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Gemini.Modules.PropertyGrid;

namespace Gemini.Demo.Modules.Home
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

        [Export]
        public static MenuItemDefinition ViewHelixMenuItem = new CommandMenuItemDefinition<ViewHelixCommandDefinition>(
            ViewDemoMenuGroup, 1);

	    public override IEnumerable<IDocument> DefaultDocuments
	    {
	        get
	        {
                yield return IoC.Get<HomeViewModel>();
                yield return IoC.Get<HelixViewModel>();
	        }
	    }

        public override void PostInitialize()
        {
            IoC.Get<IPropertyGrid>().SelectedObject = IoC.Get<HomeViewModel>();
            Shell.OpenDocument(IoC.Get<HomeViewModel>());
        }
	}
}