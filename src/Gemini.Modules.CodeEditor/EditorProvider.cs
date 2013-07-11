using System.ComponentModel.Composition;
using System.IO;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.CodeEditor.ViewModels;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Gemini.Modules.CodeEditor
{
	[Export(typeof(IEditorProvider))]
	public class EditorProvider : IEditorProvider
	{
		public bool Handles(string path)
		{
			var extension = Path.GetExtension(path);

		    return HighlightingManager.Instance.GetDefinitionByExtension(extension) != null;
		}

		public IDocument Create(string path)
		{
			var editor = new CodeEditorViewModel();
			editor.Open(path);
			return editor;
		}
	}
}