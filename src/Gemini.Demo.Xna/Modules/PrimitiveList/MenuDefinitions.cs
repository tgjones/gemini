using System.ComponentModel.Composition;
using Gemini.Demo.Xna.Modules.PrimitiveList.Commands;
using Gemini.Framework.Menus;

namespace Gemini.Demo.Xna.Modules.PrimitiveList
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition ViewPrimitiveListMenuItem = new CommandMenuItemDefinition<ViewPrimitiveListCommandDefinition>(
            Startup.Module.DemosMenuGroup, 0);
    }
}