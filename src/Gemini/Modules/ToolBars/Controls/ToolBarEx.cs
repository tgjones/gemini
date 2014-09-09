using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Gemini.Framework.Controls;
using Gemini.Modules.ToolBars.Models;

namespace Gemini.Modules.ToolBars.Controls
{
    public class ToolBarEx : System.Windows.Controls.ToolBar
    {
        private object _currentItem;

        public ToolBarEx()
        {
            SetResourceReference(StyleProperty, typeof(ToolBar));
        }

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
                return CreateButton<ToggleButton>(ToggleButtonStyleKey, "ToolBarToggleButton");

            return CreateButton<Button>(ButtonStyleKey, "ToolBarButton");
        }

        private static T CreateButton<T>(object baseStyleKey, string styleKey)
            where T : FrameworkElement, new()
        {
            var result = new T();
            result.SetResourceReference(DynamicStyle.BaseStyleProperty, baseStyleKey);
            result.SetResourceReference(DynamicStyle.DerivedStyleProperty, styleKey);
            return result;
        }
    }
}