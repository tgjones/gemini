using ICSharpCode.AvalonEdit;

namespace Gemini.Modules.CodeEditor.Views
{
    public interface ICodeEditorView
    {
        TextEditor TextEditor { get; }

        void ApplySettings();
    }
}
