using System;
using System.ComponentModel.Composition;
using System.IO;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.CodeEditor.ViewModels;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Gemini.Modules.CodeEditor
{
	[Export(typeof(IEditorProvider))]
	public class EditorProvider : IEditorProvider
	{
	    private readonly HighlightingManager _highlightingManager;

	    [ImportingConstructor]
        public EditorProvider(HighlightingManager highlightingManager)
        {
            _highlightingManager = highlightingManager;
        }

        public bool Handles(string path)
		{
			var extension = Path.GetExtension(path);

            return extension != null && _highlightingManager.GetDefinitionByExtension(extension) != null;
		}

		public IDocument Create(string path)
		{
            var editor = IoC.Get<CodeEditorViewModel>();
			editor.Open(path);
			return editor;
		}
	}
}