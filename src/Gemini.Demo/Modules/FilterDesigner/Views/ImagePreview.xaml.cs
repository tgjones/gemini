using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gemini.Demo.Modules.FilterDesigner.Views
{
    /// <summary>
    /// Interaction logic for ImagePreview.xaml
    /// </summary>
    public partial class ImagePreview : UserControl
    {
        //public static readonly DependencyProperty ElementsSourceProperty = DependencyProperty.Register(
        //    "ElementsSource", typeof(Im), typeof(GraphControl));

        //public IEnumerable ElementsSource
        //{
        //    get { return (IEnumerable)GetValue(ElementsSourceProperty); }
        //    set { SetValue(ElementsSourceProperty, value); }
        //}



        public ImagePreview()
        {
            InitializeComponent();
        }
    }
}
