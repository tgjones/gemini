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

		public bool Handles(string path)
		{
			var extension = Path.GetExtension(path);
			return _extensions.Contains(extension);
		}

		public IDocument Create(string path)
		{
			var editor = new EditorViewModel();
			editor.Open(path);
			return editor;
		}
	}
}