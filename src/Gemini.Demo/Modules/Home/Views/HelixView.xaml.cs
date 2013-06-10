using System.Windows.Controls;

namespace Gemini.Demo.Modules.Home.Views
{
    /// <summary>
    /// Interaction logic for HelixView.xaml
    /// </summary>
    public partial class HelixView : UserControl, IHelixView
    {
        public ICSharpCode.AvalonEdit.TextEditor TextEditor
        {
            get { return CodeEditor; }
        }

        public HelixView()
        {
            InitializeComponent();
        }
    }
}
