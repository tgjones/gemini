using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Gemini.Demo.Modules.TextEditor.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Gemini.Demo.Modules.TextEditor
{
	[Export(typeof(IEditorProvider))]
	public class EditorProvider : IEditorProvider
	{
		private readonly List<string> _extensions = new List<string>
        {
            ".txt",
            ".cmd"
        };

        public IEnumerable<EditorFileType> FileTypes
        {
            get { yield return new EditorFileType("Text File", ".txt"); }
        }

		public bool Handles(string path)
		{
			var extension = Path.GetExtension(path);
			return _extensions.Contains(extension);
		}

        public IDocument CreateNew(string name)
        {
            var editor = new EditorViewModel();
            editor.New(name);
            return editor;
        }

		public IDocument Open(string path)
		{
			var editor = new EditorViewModel();
			editor.Open(path);
			return editor;
		}
	}
}