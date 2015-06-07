using System.ComponentModel.Composition;
using System.IO;
using Gemini.Framework;
using Gemini.Modules.CodeEditor.Views;

namespace Gemini.Modules.CodeEditor.ViewModels
{
    [Export(typeof(CodeEditorViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
#pragma warning disable 659
    public class CodeEditorViewModel : Document
#pragma warning restore 659
    {
        private readonly LanguageDefinitionManager _languageDefinitionManager;
        private string _originalText;
        private string _path;
        private string _fileName;
        private bool _isDirty;
        private ICodeEditorView _view;

        [ImportingConstructor]
        public CodeEditorViewModel(LanguageDefinitionManager languageDefinitionManager)
        {
            _languageDefinitionManager = languageDefinitionManager;
        }

        public string Path
        {
            get { return _path; }
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
                UpdateDisplayName();
            }
        }

        public override bool ShouldReopenOnStart
        {
            get { return true; }
        }

        public override void CanClose(System.Action<bool> callback)
        {
            callback(!IsDirty);
        }

        public void New(string name)
        {
            _fileName = name;
            UpdateDisplayName();
        }

        public void Open(string path)
        {
            _path = path;
            _fileName = System.IO.Path.GetFileName(_path);
            UpdateDisplayName();
        }

        public override void SaveState(BinaryWriter writer)
        {
            writer.Write(_path);
        }

        public override void LoadState(BinaryReader reader)
        {
            Open(reader.ReadString());
        }

        private void UpdateDisplayName()
        {
            DisplayName = (IsDirty) ? _fileName + "*" : _fileName;
        }

        protected override void OnViewLoaded(object view)
        {
            if (_path != null)
                using (var stream = File.OpenText(_path))
                    _originalText = stream.ReadToEnd();

            _view = (ICodeEditorView) view;
            _view.TextEditor.Text = _originalText;

            _view.TextEditor.TextChanged += delegate
            {
                IsDirty = string.Compare(_originalText, _view.TextEditor.Text) != 0;
            };

            var fileExtension = System.IO.Path.GetExtension(_fileName).ToLower();

            ILanguageDefinition languageDefinition = _languageDefinitionManager.GetDefinitionByExtension(fileExtension);

            SetLanguage(languageDefinition);
        }

        public override bool Equals(object obj)
        {
            var other = obj as CodeEditorViewModel;
            return other != null && string.Compare(_path, other._path) == 0 &&
                (_path != null || _fileName == other._fileName);
        }

        public void Save()
        {
            var newText = _view.TextEditor.Text;
            File.WriteAllText(_path, newText);
            _originalText = newText;

            IsDirty = false;
        }

        private void SetLanguage(ILanguageDefinition languageDefinition)
        {
            if (languageDefinition == null)
            {
                _view.TextEditor.SyntaxHighlighting = null;
            }
            else
            {
                _view.TextEditor.SyntaxHighlighting = languageDefinition.SyntaxHighlighting;
            }
        }
    }
}