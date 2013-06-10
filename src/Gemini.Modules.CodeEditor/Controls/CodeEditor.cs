using System.Windows.Media;
using ICSharpCode.AvalonEdit;

namespace Gemini.Modules.CodeEditor.Controls
{
    public class CodeEditor : TextEditor
    {
        public CodeEditor()
        {
            FontFamily = new FontFamily("Consolas");
            FontSize = 12;
            ShowLineNumbers = true;
            Options = new TextEditorOptions
            {
                ConvertTabsToSpaces = true
            };
        }
    }
}