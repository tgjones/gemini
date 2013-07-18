using System.ComponentModel.Composition;
using System.IO;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.CodeEditor.ViewModels;

namespace Gemini.Modules.CodeEditor
{
	[Export(typeof(IEditorProvider))]
	public class EditorProvider : IEditorProvider
	{
	    private readonly LanguageDefinitionManager _languageDefinitionManager;

        [ImportingConstructor]
	    public EditorProvider(LanguageDefinitionManager languageDefinitionManager)
	    {
	        _languageDefinitionManager = languageDefinitionManager;
	    }

	    public bool Handles(string path)
		{
			var extension = Path.GetExtension(path);

            return extension != null && _languageDefinitionManager.GetDefinitionByExtension(extension) != null;
		}

		public IDocument Create(string path)
		{
            var editor = IoC.Get<CodeEditorViewModel>();
			editor.Open(path);
			return editor;
		}
	}
}