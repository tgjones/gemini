using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gemini.Modules.UndoRedo.ViewModels;

namespace Gemini.Modules.UndoRedo.Views
{
    /// <summary>
    /// Interaction logic for UndoListView.xaml
    /// </summary>
    public partial class UndoListView : UserControl
    {
        public UndoListView()
        {
            InitializeComponent();
        }

        private void ItemsMouseMove(object sender, MouseEventArgs e)
        {
            var result = VisualTreeHelper.HitTest(Items, e.GetPosition(Items));
            var hoveredItem = result.VisualHit as FrameworkElement;
            var undoableAction = (IUndoableAction) hoveredItem.DataContext;

            var viewModel = (UndoListViewModel) DataContext;
            viewModel.HoveredIndex = viewModel.Items.IndexOf(undoableAction);
        }
    }
}
