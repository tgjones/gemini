using System.Windows.Controls;
using ICSharpCode.AvalonEdit;

namespace Gemini.Modules.CodeEditor.Views
{
    /// <summary>
    /// Interaction logic for CodeEditorView.xaml
    /// </summary>
    public partial class CodeEditorView : UserControl, ICodeEditorView
    {
        public TextEditor TextEditor
        {
            get { return CodeEditor; }
        }

        public CodeEditorView()
        {
            InitializeComponent();
        }
    }
}
