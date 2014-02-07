using System;
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
        private IInspectorTool _inspectorTool;

		[Import]
		private IResourceManager _resourceManager;

        public override IEnumerable<Type> DefaultTools
        {
            get { yield return typeof(IInspectorTool); }
        }

		public override void Initialize()
		{
            Shell.ToolBars.Visible = true;
		    Shell.ToolBars.Items.Add(new ToolBarModel
		    {
		        new ToolBarItem("Open", OpenFile).WithIcon(typeof(ModuleBase).Assembly),
                ToolBarItemBase.Separator,
                new UndoToolBarItem(),
                new RedoToolBarItem()
		    });

            MainWindow.WindowState = WindowState.Maximized;
            MainWindow.Title = "Gemini Demo";

            Shell.StatusBar.AddItem("Hello world!", new GridLength(1, GridUnitType.Star));
            Shell.StatusBar.AddItem("Ln 44", new GridLength(100));
            Shell.StatusBar.AddItem("Col 79", new GridLength(100));

            MainWindow.Icon = _resourceManager.GetBitmap("Resources/Icon.png", 
				Assembly.GetExecutingAssembly().GetAssemblyName());

			_output.AppendLine("Started up");

            MainMenu.All.First(x => x.Name == "View")
                .Add(new MenuItem("History", OpenHistory));

		    Shell.ActiveDocumentChanged += (sender, e) => RefreshInspector();
		    RefreshInspector();
		}

        private void RefreshInspector()
        {
            if (Shell.ActiveItem != null)
                _inspectorTool.SelectedObject = new InspectableObjectBuilder()
                       .WithObjectProperties(Shell.ActiveItem, pd => true)
                       .ToInspectableObject();
            else
                _inspectorTool.SelectedObject = null;
        }

        private IEnumerable<IResult> OpenFile()
        {
            var dialog = new OpenFileDialog();
            yield return Show.CommonDialog(dialog);
            yield return Show.Document(dialog.FileName);
        }

        private static IEnumerable<IResult> OpenHistory()
        {
            yield return Show.Tool<IHistoryTool>();
        }
	}
}