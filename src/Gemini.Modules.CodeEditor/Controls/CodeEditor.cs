using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Search;

namespace Gemini.Modules.CodeEditor.Controls
{
    public class CodeEditor : TextEditor
    {
        #region LineNumber

        /// <summary>
        /// The <see cref="LineNumber"/> property.
        /// </summary>
        public static DependencyProperty LineNumberProperty = DependencyProperty.Register(
            "LineNumber",
            typeof(int),
            typeof(CodeEditor),
            new PropertyMetadata(default(int)));

        public int LineNumber
        {
            get { return (int)GetValue(LineNumberProperty); }
            set { SetValue(LineNumberProperty, value); }
        }

        #endregion LineNumber

        #region ColumnPosition

        /// <summary>
        /// The <see cref="ColumnPosition"/> property.
        /// </summary>
        public static DependencyProperty ColumnPositionProperty = DependencyProperty.Register(
            "ColumnPosition",
            typeof(int),
            typeof(CodeEditor),
            new PropertyMetadata(default(int)));

        public int ColumnPosition
        {
            get { return (int)GetValue(ColumnPositionProperty); }
            set { SetValue(ColumnPositionProperty, value); }
        }

        #endregion ColumnPosition

        public CodeEditor()
        {
            ApplySettings();
            
            // Caret related
            LineNumber = 1;
            ColumnPosition = 1;
            TextArea.Caret.PositionChanged += OnCaretPositionChanged;
        }

        public void ApplySettings()
        {
            FontFamily = new FontFamily("Consolas");
            FontSize = 14;
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

        #region Event Handlers

        private void OnCaretPositionChanged(object sender, System.EventArgs e)
        {
            LineNumber = TextArea.Caret.Line;
            ColumnPosition = TextArea.Caret.Column;
        }

        #endregion Event Handlers
    }
}
