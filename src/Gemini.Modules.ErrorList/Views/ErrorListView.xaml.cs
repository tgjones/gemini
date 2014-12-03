using System.Windows.Controls;
using System.Windows.Input;

namespace Gemini.Modules.ErrorList.Views
{
    /// <summary>
    /// Interaction logic for ErrorListView.xaml
    /// </summary>
    public partial class ErrorListView : UserControl
    {
        public ErrorListView()
        {
            InitializeComponent();
        }

        private void OnDataGridMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = (DataGrid) sender;
            if (dataGrid.SelectedItems == null || dataGrid.SelectedItems.Count != 1)
                return;

            var errorListItem = (ErrorListItem) dataGrid.SelectedItem;
            if (errorListItem.OnClick != null)
                errorListItem.OnClick();
        }
    }
}
