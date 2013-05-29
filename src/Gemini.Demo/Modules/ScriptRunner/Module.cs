using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Demo.Modules.ScriptRunner.Results;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Gemini.Modules.CodeCompiler;
using Gemini.Modules.CodeEditor.ViewModels;

namespace Gemini.Demo.Modules.ScriptRunner
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
        [Import]
	    private ICodeCompiler _codeCompiler;

		public override void Initialize()
		{
		    var scriptMenuItem = new MenuItem("Script")
		    {
		        new MenuItem("Compile", CompileScript)
		    };
		    MainMenu.Add(scriptMenuItem);
		}

        private IEnumerable<IResult> CompileScript()
        {
            var codeEditorViewModel = Shell.ActiveItem as CodeEditorViewModel;
            if (codeEditorViewModel == null)
                yield break;

            codeEditorViewModel.Save();

			yield return new CompileCodeResult(_codeCompiler,
                codeEditorViewModel.Path);
		}
	}
}