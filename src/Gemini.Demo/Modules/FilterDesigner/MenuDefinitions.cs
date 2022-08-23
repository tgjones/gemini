using System.ComponentModel.Composition;
using Gemini.Demo.Modules.FilterDesigner.Commands;
using Gemini.Framework.Menus;

namespace Gemini.Demo.Modules.FilterDesigner
{
    public static class MenuDefinitions
    {
        [Export]
        public static readonly MenuItemDefinition OpenGraphMenuItem = new CommandMenuItemDefinition<OpenGraphCommandDefinition>(
            Gemini.Modules.MainMenu.MenuDefinitions.FileNewOpenMenuGroup, 2);
    }
}
