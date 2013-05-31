using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Framework.Services;
using Gemini.Framework.ToolBars;
using Gemini.Modules.Inspector;
using Gemini.Modules.Output;
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
		    Shell.ToolBars.Add(new ToolBarModel
		    {
		        new ToolBarItem("Open", OpenFile).WithIcon(typeof(ModuleBase).Assembly),
                ToolBarItemBase.Separator,
                UndoRedoToolBarItems.CreateUndoToolbarItem(),
                UndoRedoToolBarItems.CreateRedoToolbarItem()
		    });

			Shell.WindowState = WindowState.Maximized;
			Shell.Title = "Gemini Demo";
			Shell.StatusBar.Message = "Hello world!";
			Shell.Icon = _resourceManager.GetBitmap("Resources/Icon.png", 
				Assembly.GetExecutingAssembly().GetAssemblyName());

			_output.AppendLine("Started up");

		    Shell.ShowTool(IoC.Get<IInspectorTool>());
		}

        private IEnumerable<IResult> OpenFile()
        {
            var dialog = new OpenFileDialog();
            yield return Show.Dialog(dialog);
            yield return Show.Document(dialog.FileName);
        }
	}
}