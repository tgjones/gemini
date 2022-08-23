using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
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
        public static readonly MenuItemGroupDefinition ViewDemoMenuGroup = new MenuItemGroupDefinition(
            Gemini.Modules.MainMenu.MenuDefinitions.ViewMenu, 10);

        [Export]
        public static readonly MenuItemDefinition ViewHomeMenuItem = new CommandMenuItemDefinition<ViewHomeCommandDefinition>(
            ViewDemoMenuGroup, 0);

        [Export]
        public static readonly MenuItemDefinition ViewHelixMenuItem = new CommandMenuItemDefinition<ViewHelixCommandDefinition>(
            ViewDemoMenuGroup, 1);

        #region Debug menu
        [Export]
        public static readonly MenuDefinition DebugMenu = new MenuDefinition(
            Gemini.Modules.MainMenu.MenuDefinitions.MainMenuBar,
            int.MaxValue,
            "DEBUG")
            // Exclude this menu when there is no debugger attached AT STARTUP.
            // This predicate isn't re-evaluated after menus are built (unless rebuild functionality gets added)
            .SetDynamicExclusionPredicate(_ => System.Diagnostics.Debugger.IsAttached==false);

        [Export]
        public static readonly MenuItemGroupDefinition DebugMenuGroup = new MenuItemGroupDefinition(
            DebugMenu, 1);

        // You should NOT see this item when there unless a debugger was attached during startup
        [Export]
        public static readonly MenuItemDefinition DebugTestMenu = new TextMenuItemDefinition(
            DebugMenuGroup, 0,
            "Debugger.IsAttached=true");
        #endregion

        public override IEnumerable<IDocument> DefaultDocuments
        {
            get
            {
                yield return IoC.Get<HomeViewModel>();
                yield return IoC.Get<HelixViewModel>();
            }
        }

        public override async Task PostInitializeAsync()
        {
            IoC.Get<IPropertyGrid>().SelectedObject = IoC.Get<HomeViewModel>();
            await Shell.OpenDocumentAsync(IoC.Get<HomeViewModel>());
        }
    }
}
