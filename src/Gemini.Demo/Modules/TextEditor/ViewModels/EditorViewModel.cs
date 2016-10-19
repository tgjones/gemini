using System;
using System.IO;
using System.Threading.Tasks;
using Gemini.Demo.Modules.TextEditor.Views;
using Gemini.Framework;
using Gemini.Framework.Threading;

namespace Gemini.Demo.Modules.TextEditor.ViewModels
{
#pragma warning disable 659
    public class EditorViewModel : PersistedDocument
#pragma warning restore 659
    {
        private EditorView _view;
		private string _originalText;

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
            var newText = _view.textBox.Text;
            File.WriteAllText(filePath, newText);
            _originalText = newText;
            return TaskUtility.Completed;
        }

        private void ApplyOriginalText()
        {
            _view.textBox.Text = _originalText;

            _view.textBox.TextChanged += delegate
            {
                IsDirty = !string.Equals(_originalText, _view.textBox.Text, StringComparison.Ordinal);
            };
        }

		protected override void OnViewLoaded(object view)
		{
            _view = (EditorView) view;
		}

        public override bool Equals(object obj)
		{
			var other = obj as EditorViewModel;
            return other != null
                && string.Equals(DocumentPath, other.DocumentPath, StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(DocumentName, other.DocumentName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
