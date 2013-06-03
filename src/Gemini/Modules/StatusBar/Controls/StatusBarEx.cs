using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Gemini.Modules.StatusBar.ViewModels;

namespace Gemini.Modules.StatusBar.Controls
{
    public class StatusBarEx : System.Windows.Controls.Primitives.StatusBar
    {
        static StatusBarEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusBarEx),
                new FrameworkPropertyMetadata(typeof(StatusBarEx)));
        }

        private Grid ItemsHost
        {
            get
            {
                return (Grid) typeof(System.Windows.Controls.Primitives.StatusBar).InvokeMember("ItemsHost",
                    BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance,
                    null, this, null);
            }
        }

        public StatusBarEx()
        {
            ((INotifyCollectionChanged) Items).CollectionChanged += (sender, e) =>
                RefreshGridColumns();
        }

        private void RefreshGridColumns()
        {
            var grid = ItemsHost;
            if (grid == null)
                return;
            grid.ColumnDefinitions.Clear();
            foreach (var item in Items.Cast<StatusBarItemViewModel>())
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = item.Width });
        }
    }
}