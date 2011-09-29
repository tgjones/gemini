using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Menus;
using Gemini.Framework.Results;
using Gemini.Framework.Services;
using Microsoft.Win32;

namespace Gemini.Modules.Shell.ViewModels
{
	[Export(typeof(IMenu))]
	public class MainMenuViewModel : MenuModel
	{
		[Import] 
		private IShell _shell;

		public MainMenuViewModel()
		{
			Add(
				new MenuItem("_File")
				{
					new MenuItem("_Open", OpenFile).WithIcon(),
					MenuItemBase.Separator,
					new MenuItem("E_xit", Exit),
				},
				new MenuItem("_View"));
		}

		private IEnumerable<IResult> OpenFile()
		{
			var dialog = new OpenFileDialog();
			yield return Show.Dialog(dialog);
			yield return Show.Document(dialog.FileName);
		}

		private IEnumerable<IResult> Exit()
		{
			_shell.Close();
			yield break;
		}
	}
}