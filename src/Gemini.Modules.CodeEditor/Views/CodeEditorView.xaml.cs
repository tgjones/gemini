using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;

namespace Gemini.Modules.CodeEditor.Views
{
    public partial class CodeEditorView : UserControl, ICodeEditorView
    {
        public TextEditor TextEditor
        {
            get { return CodeEditor; }
        }

        public CodeEditorView()
        {
            InitializeComponent();
            Loaded += (sender, e) => MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        public void ApplySettings()
        {
            CodeEditor?.ApplySettings();
        }
    }
}
