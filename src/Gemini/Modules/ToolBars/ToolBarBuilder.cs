using System.ComponentModel.Composition;
using System.Linq;
using Gemini.Framework.Commands;
using Gemini.Framework.ToolBars;
using Gemini.Modules.ToolBars.Models;

namespace Gemini.Modules.ToolBars
{
    [Export(typeof(IToolBarBuilder))]
    public class ToolBarBuilder : IToolBarBuilder
    {
        [Import]
        private ICommandService _commandService;

        [ImportMany]
        private ToolBarDefinition[] _toolBars;

        [ImportMany]
        private ToolBarItemGroupDefinition[] _toolBarItemGroups;

        [ImportMany]
        private ToolBarItemDefinition[] _toolBarItems;

        public void BuildToolBars(IToolBars result)
        {
            var toolBars = _toolBars.OrderBy(x => x.SortOrder);

            foreach (var toolBar in toolBars)
            {
                var toolBarModel = new ToolBarModel();
                BuildToolBar(toolBar, toolBarModel);
                if (toolBarModel.Any())
                    result.Items.Add(toolBarModel);
            }
        }

        public void BuildToolBar(ToolBarDefinition toolBarDefinition, IToolBar result)
        {
            var groups = _toolBarItemGroups
                .Where(x => x.ToolBar == toolBarDefinition)
                .OrderBy(x => x.SortOrder)
                .ToList();

            for (int i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var toolBarItems = _toolBarItems
                    .Where(x => x.Group == group)
                    .OrderBy(x => x.SortOrder);

                foreach (var toolBarItem in toolBarItems)
                    result.Add(new CommandToolBarItem(_commandService.GetCommand(toolBarItem.CommandDefinition), result));

                if (i < groups.Count - 1 && toolBarItems.Any())
                    result.Add(new ToolBarItemSeparator());
            }
        }
    }
}