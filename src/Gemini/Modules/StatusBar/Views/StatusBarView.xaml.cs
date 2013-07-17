using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Gemini.Modules.StatusBar.ViewModels;

namespace Gemini.Modules.StatusBar.Views
{
    /// <summary>
    /// Interaction logic for StatusBarView.xaml
    /// </summary>
    public partial class StatusBarView : UserControl
    {
        private Grid _statusBarGrid;

        public StatusBarView()
        {
            InitializeComponent();
        }

        private void OnStatusBarGridLoaded(object sender, RoutedEventArgs e)
        {
            _statusBarGrid = (Grid) sender;
            RefreshGridColumns();
        }

        private void RefreshGridColumns()
        {
            _statusBarGrid.ColumnDefinitions.Clear();
            foreach (var item in StatusBar.Items.Cast<StatusBarItemViewModel>())
                _statusBarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = item.Width });
        }
    }
}
