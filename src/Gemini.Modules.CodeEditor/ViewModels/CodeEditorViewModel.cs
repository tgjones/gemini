using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using Gemini.Framework;
using Gemini.Framework.Threading;
using Gemini.Modules.CodeEditor.Views;
using Gemini.Modules.StatusBar;
using System.ComponentModel;
using Caliburn.Micro;

namespace Gemini.Modules.CodeEditor.ViewModels
{
    [Export(typeof(CodeEditorViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
#pragma warning disable 659
    public class CodeEditorViewModel : PersistedDocument
#pragma warning restore 659
    {
        private readonly LanguageDefinitionManager _languageDefinitionManager;
        private string _originalText;
        private ICodeEditorView _view;
        private IStatusBar _statusBar;
        private bool notYetLoaded = false;

        [ImportingConstructor]
        public CodeEditorViewModel(LanguageDefinitionManager languageDefinitionManager)
        {
            _languageDefinitionManager = languageDefinitionManager;
        }

        protected override void OnViewLoaded(object view)
        {
            _view = (ICodeEditorView) view;
            _statusBar = IoC.Get<IStatusBar>();

            if (notYetLoaded)
            {
                ApplyOriginalText();
                notYetLoaded = false;
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as CodeEditorViewModel;
            return other != null
                && string.Equals(FilePath, other.FilePath, StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(FileName, other.FileName, StringComparison.InvariantCultureIgnoreCase);
        }

        protected override Task DoNew()
        {
            _originalText = string.Empty;
            ApplyOriginalText();
            return TaskUtility.Completed;
        }

        protected override Task DoLoad(string filePath)
        {
            _originalText = File.ReadAllText(filePath);
            ApplyOriginalText();
            return TaskUtility.Completed;
        }

        protected override Task DoSave(string filePath)
        {
            var newText = _view.TextEditor.Text;
            File.WriteAllText(filePath, newText);
            _originalText = newText;
            return TaskUtility.Completed;
        }

        private void ApplyOriginalText()
        {
            // At StartUp, _view is null, so notYetLoaded flag is added
            if (_view == null)
            {
                notYetLoaded = true;
                return;
            }
            _view.TextEditor.Text = _originalText;

            _view.TextEditor.TextChanged += delegate
            {
                IsDirty = string.Compare(_originalText, _view.TextEditor.Text) != 0;
            };

            UpdateStatusBar();

            // To update status bar items, Caret PositionChanged event is added
            _view.TextEditor.TextArea.Caret.PositionChanged += delegate
            {
                UpdateStatusBar();
            };

            var fileExtension = Path.GetExtension(FileName).ToLower();

            ILanguageDefinition languageDefinition = _languageDefinitionManager.GetDefinitionByExtension(fileExtension);

            SetLanguage(languageDefinition);
        }

        /// <summary>
        /// Update Column and Line position properties when caret position is changed
        /// </summary>
        private void UpdateStatusBar()
        {
            int lineNumber = _view.TextEditor.Document.GetLineByOffset(_view.TextEditor.CaretOffset).LineNumber;
            int colPosition = _view.TextEditor.TextArea.Caret.VisualColumn + 1;

            // TODO: Now I don't know about Ch#
            //int charPosition = _view.TextEditor.CaretOffset;

            if (_statusBar != null && _statusBar.Items.Count >= 3)
            {
                _statusBar.Items[1].Message = string.Format("Ln {0}", lineNumber);
                _statusBar.Items[2].Message = string.Format("Col {0}", colPosition);
            }
        }

        private void SetLanguage(ILanguageDefinition languageDefinition)
        {
            _view.TextEditor.SyntaxHighlighting = (languageDefinition != null)
                ? languageDefinition.SyntaxHighlighting
                : null;
        }
    }
}