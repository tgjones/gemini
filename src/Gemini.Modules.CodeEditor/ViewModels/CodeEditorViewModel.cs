using System;
using System.ComponentModel.Composition;
using System.IO;
using Gemini.Framework;
using Gemini.Modules.CodeEditor.Views;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Gemini.Modules.CodeEditor.ViewModels
{
    [Export(typeof(CodeEditorViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CodeEditorViewModel : Document
    {
        private readonly HighlightingManager _highlightingManager;
        private string _originalText;
        private string _path;
        private string _fileName;
        private bool _isDirty;
        private ICodeEditorView _view;

        [ImportingConstructor]
        public CodeEditorViewModel(HighlightingManager highlightingManager)
        {
            _highlightingManager = highlightingManager;
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

        public override void CanClose(System.Action<bool> callback)
        {
            callback(!IsDirty);
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
            using (var stream = File.OpenText(_path))
                _originalText = stream.ReadToEnd();

            _view = (ICodeEditorView) view;
            _view.TextEditor.Text = _originalText;

            _view.TextEditor.TextChanged += delegate
            {
                IsDirty = string.Compare(_originalText, _view.TextEditor.Text) != 0;
            };

            var fileExtension = System.IO.Path.GetExtension(_fileName).ToLower();
            var highlightingDefinition = _highlightingManager.GetDefinitionByExtension(fileExtension);
            _view.TextEditor.SyntaxHighlighting = highlightingDefinition;
        }

        public override bool Equals(object obj)
        {
            var other = obj as CodeEditorViewModel;
            return other != null && string.Compare(_path, other._path) == 0;
        }

        public void Save()
        {
            var newText = _view.TextEditor.Text;
            File.WriteAllText(_path, newText);
            _originalText = newText;

            IsDirty = false;
        }
    }
}