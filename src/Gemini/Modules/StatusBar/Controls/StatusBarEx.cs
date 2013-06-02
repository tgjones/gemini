using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;

namespace Gemini.Modules.StatusBar.Controls
{
    public class StatusBarEx : System.Windows.Controls.Primitives.StatusBar
    {
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
            grid.ColumnDefinitions.Clear();
            foreach (var item in Items.Cast<StatusBarItemViewModel>())
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = item.Width });
        }
    }
}