using System.IO;
using Gemini.Demo.Modules.TextEditor.Views;
using Gemini.Framework;

namespace Gemini.Demo.Modules.TextEditor.ViewModels
{
	public class EditorViewModel : Document
	{
		private string _originalText;
		private string _path;
		private string _fileName;
		private bool _isDirty;

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
			_fileName = Path.GetFileName(_path);
		    UpdateDisplayName();
		}

        private void UpdateDisplayName()
        {
            DisplayName = (IsDirty) ? _fileName + "*" : _fileName;
        }

		protected override void OnViewLoaded(object view)
		{
			using (var stream = File.OpenText(_path))
			{
				_originalText = stream.ReadToEnd();
			}

			var editor = (EditorView) view;
			editor.textBox.Text = _originalText;

			editor.textBox.TextChanged += delegate
			{
				IsDirty = string.Compare(_originalText, editor.textBox.Text) != 0;
			};
		}

		public override bool Equals(object obj)
		{
			var other = obj as EditorViewModel;
			return other != null && string.Compare(_path, other._path) == 0;
		}
	}
}