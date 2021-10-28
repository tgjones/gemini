using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Search;

namespace Gemini.Modules.CodeEditor.Controls
{
    public class CodeEditor : TextEditor
    {
        public CodeEditor()
        {
            ApplySettings();
            Loaded += (s, e) => SearchPanel.Install(this);
            DocumentChanged += (s, e) => SearchPanel.Install(this);
        }

        public void ApplySettings()
        {
            FontFamily = new FontFamily("Consolas");
            FontSize = 12;
            ShowLineNumbers = Properties.Settings.Default.ShowLineNumbers;
            WordWrap = Properties.Settings.Default.WordWrap;

            if (Options == null)
            {
                Options = new TextEditorOptions
                {
                    ConvertTabsToSpaces = true,
                    ShowEndOfLine = Properties.Settings.Default.ShowEndOfLine,
                    ShowSpaces = Properties.Settings.Default.ShowSpaces,
                    ShowTabs = Properties.Settings.Default.ShowTabs,
                };
            }
            else
            {
                Options.ShowEndOfLine = Properties.Settings.Default.ShowEndOfLine;
                Options.ShowSpaces = Properties.Settings.Default.ShowSpaces;
                Options.ShowTabs = Properties.Settings.Default.ShowTabs;
            }
        }
    }
}
