using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Gemini.Modules.ToolBars.Models;

namespace Gemini.Modules.ToolBars.Controls
{
    public class ToolBarEx : System.Windows.Controls.ToolBar
    {
        private object _currentItem;

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            _currentItem = item;
            return base.IsItemItsOwnContainerOverride(item);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            if (_currentItem is ToolBarItemSeparator)
                return new Separator();

            if (_currentItem is ToggleToolBarItem)
                return new ToggleButton
                {
                    Style = (Style) FindResource("ToolBarToggleButton")
                };

            return new Button
            {
                Style = (Style) FindResource("ToolBarButton")
            };
        }
    }
}