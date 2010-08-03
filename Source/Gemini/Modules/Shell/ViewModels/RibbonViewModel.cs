using System.Collections.Generic;
using Gemini.Framework.Results;
using Gemini.Framework.Ribbon;
using Gemini.Framework.Services;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Caliburn.PresentationFramework;

namespace Gemini.Modules.Shell.ViewModels
{
	public class RibbonViewModel : RibbonModel
	{
		private readonly IServiceLocator _serviceLocator;

		public RibbonViewModel(IServiceLocator serviceLocator)
		{
			_serviceLocator = serviceLocator;

			AddBackstageItems(
				new RibbonButton("Open", OpenFile).WithIcon(),
				new RibbonButton("Exit", Exit).WithIcon()
			);

			AddTabs(
				new RibbonTab("Home", new List<RibbonGroup>
				{
					new RibbonGroup("Tools"),
					new RibbonGroup("Help", new List<IRibbonItem>
					{
						new RibbonButton("About", About).WithIcon()
					})
				})
			);
		}

		private IEnumerable<IResult> OpenFile()
		{
			var dialog = new OpenFileDialog();
			yield return Show.Dialog(dialog);

			yield return Show.Document(dialog.FileName);
		}

		private IEnumerable<IResult> Exit()
		{
			_serviceLocator.GetInstance<IShell>().Close();
			yield break;
		}

		private IEnumerable<IResult> About()
		{
			yield return Show.MessageBox("This is the MDI Shell Sample.");
		}
	}
}