using System.ComponentModel.Composition;
using Gemini.Framework.ToolBars;

namespace Gemini.Modules.ToolBars
{
    internal static class ToolBarDefinitions
    {
        [Export]
        public static ToolBarDefinition StandardToolBar = new ToolBarDefinition(0, "Standard");
    }
}