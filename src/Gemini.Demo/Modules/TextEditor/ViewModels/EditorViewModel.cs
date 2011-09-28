using System.IO;
using AvalonDock;
using Caliburn.Micro;
using Gemini.Demo.Modules.TextEditor.Views;

namespace Gemini.Demo.Modules.TextEditor.ViewModels
{
	public class EditorViewModel : Screen
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
			{
				_originalText = stream.ReadToEnd();
			}

			var editor = (EditorView)((DocumentContent)view).Content;
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