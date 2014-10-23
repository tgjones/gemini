using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
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

	    public IEnumerable<EditorFileType> FileTypes
	    {
	        get
	        {
	            return _languageDefinitionManager.LanguageDefinitions
	                .Select(languageDefinition => new EditorFileType
	                {
	                    Name = languageDefinition.Name + " File",
	                    FileExtension = languageDefinition.FileExtensions.First()
	                });
	        }
	    }

	    public IDocument CreateNew(string name)
	    {
            var editor = IoC.Get<CodeEditorViewModel>();
            editor.New(name);
            return editor;
	    }

	    public bool Handles(string path)
		{
			var extension = Path.GetExtension(path);
            return extension != null && _languageDefinitionManager.GetDefinitionByExtension(extension) != null;
		}

		public IDocument Open(string path)
		{
            var editor = IoC.Get<CodeEditorViewModel>();
			editor.Open(path);
			return editor;
		}
	}
}