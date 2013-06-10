using System.Windows.Controls;

namespace Gemini.Demo.Modules.Home.Views
{
    /// <summary>
    /// Interaction logic for CubeView.xaml
    /// </summary>
    public partial class CubeView : UserControl, ICubeView
    {
        public ICSharpCode.AvalonEdit.TextEditor TextEditor
        {
            get { return CodeEditor; }
        }

        public CubeView()
        {
            InitializeComponent();
        }
    }
}
