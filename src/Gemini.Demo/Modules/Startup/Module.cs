using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Framework.Services;
using Gemini.Modules.Inspector;
using Gemini.Modules.MainMenu.Models;
using Gemini.Modules.Output;
using Gemini.Modules.StatusBar.ViewModels;
using Gemini.Modules.ToolBars.Models;
using Gemini.Modules.UndoRedo;
using Microsoft.Win32;

namespace Gemini.Demo.Modules.Startup
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
		[Import]
		private IOutput _output;

		[Import]
		private IResourceManager _resourceManager;

		public override void Initialize()
		{
            Shell.ToolBars.Visible = true;
		    Shell.ToolBars.Items.Add(new ToolBarModel
		    {
		        new ToolBarItem("Open", OpenFile).WithIcon(typeof(ModuleBase).Assembly),
                ToolBarItemBase.Separator,
                UndoRedoToolBarItems.CreateUndoToolbarItem(),
                UndoRedoToolBarItems.CreateRedoToolbarItem()
		    });

			Shell.WindowState = WindowState.Maximized;
			Shell.Title = "Gemini Demo";

            Shell.StatusBar.Items.Add(new StatusBarItemViewModel("Hello world!", new GridLength(1, GridUnitType.Star)));
            Shell.StatusBar.Items.Add(new StatusBarItemViewModel("Ln 44", new GridLength(100)));
            Shell.StatusBar.Items.Add(new StatusBarItemViewModel("Col 79", new GridLength(100)));

			Shell.Icon = _resourceManager.GetBitmap("Resources/Icon.png", 
				Assembly.GetExecutingAssembly().GetAssemblyName());

			_output.AppendLine("Started up");

		    Shell.ShowTool(IoC.Get<IInspectorTool>());

            MainMenu.All.First(x => x.Name == "View")
                .Add(new MenuItem("History", OpenHistory));
		}

        private IEnumerable<IResult> OpenFile()
        {
            var dialog = new OpenFileDialog();
            yield return Show.Dialog(dialog);
            yield return Show.Document(dialog.FileName);
        }

        private static IEnumerable<IResult> OpenHistory()
        {
            yield return Show.Tool<IHistoryTool>();
        }
	}
}