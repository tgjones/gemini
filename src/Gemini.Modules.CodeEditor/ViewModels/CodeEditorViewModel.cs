using System.IO;
using Gemini.Framework;
using Gemini.Modules.CodeEditor.Views;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Gemini.Modules.CodeEditor.ViewModels
{
    public class CodeEditorViewModel : Document
    {
        private string _originalText;
        private string _path;
        private string _fileName;
        private bool _isDirty;

        public override string DisplayName
        {
            get
            {
                if (IsDirty)
                    return _fileName + "*";
                return _fileName;
            }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (value == _isDirty)
                    return;

                _isDirty = value;
                NotifyOfPropertyChange(() => IsDirty);
                NotifyOfPropertyChange(() => DisplayName);
            }
        }

        public override void CanClose(System.Action<bool> callback)
        {
            callback(!IsDirty);
        }

        public void Open(string path)
        {
            _path = path;
            _fileName = Path.GetFileName(_path);
        }

        protected override void OnViewLoaded(object view)
        {
            using (var stream = File.OpenText(_path))
                _originalText = stream.ReadToEnd();

            var editor = (ICodeEditorView) view;
            editor.TextEditor.Text = _originalText;

            editor.TextEditor.TextChanged += delegate
            {
                IsDirty = string.Compare(_originalText, editor.TextEditor.Text) != 0;
            };

            var fileExtension = Path.GetExtension(_fileName).ToLower();
            var highlightingDefinition = HighlightingManager.Instance.GetDefinitionByExtension(fileExtension);
            editor.TextEditor.SyntaxHighlighting = highlightingDefinition;
        }

        public override bool Equals(object obj)
        {
            var other = obj as CodeEditorViewModel;
            return other != null && string.Compare(_path, other._path) == 0;
        }
    }
}