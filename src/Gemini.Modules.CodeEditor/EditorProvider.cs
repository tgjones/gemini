using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.CodeEditor.ViewModels;

namespace Gemini.Modules.CodeEditor
{
	[Export(typeof(IEditorProvider))]
	public class EditorProvider : IEditorProvider
	{
		private readonly List<string> _extensions = new List<string>
        {
            ".cs"
        };

		public bool Handles(string path)
		{
			var extension = Path.GetExtension(path);
			return _extensions.Contains(extension);
		}

		public IDocument Create(string path)
		{
			var editor = new CodeEditorViewModel();
			editor.Open(path);
			return editor;
		}
	}
}