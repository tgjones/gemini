using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Results;
using Gemini.Framework.ToolBars;
using Microsoft.Win32;

namespace Gemini.Modules.Shell.ViewModels
{
	[Export(typeof(IToolBar))]
	public class ToolBarViewModel : ToolBarModel
	{
        public ToolBarViewModel()
		{
			Add(new ToolBarItem("Open", OpenFile).WithIcon());
		}

		private IEnumerable<IResult> OpenFile()
		{
			var dialog = new OpenFileDialog();
			yield return Show.Dialog(dialog);
			yield return Show.Document(dialog.FileName);
		}
	}
}