using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using Gemini.Framework;
using Gemini.Modules.Inspector;
using Gemini.Modules.Output;

namespace Gemini.Demo.Modules.Startup
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
		private readonly IOutput _output;
        private readonly IInspectorTool _inspectorTool;

        public override IEnumerable<Type> DefaultTools
        {
            get { yield return typeof(IInspectorTool); }
        }

        [ImportingConstructor]
	    public Module(IOutput output, IInspectorTool inspectorTool)
        {
            _output = output;
            _inspectorTool = inspectorTool;
        }

	    public override void Initialize()
		{
		    Shell.ShowFloatingWindowsInTaskbar = true;
            Shell.ToolBars.Visible = true;

            //MainWindow.WindowState = WindowState.Maximized;
            MainWindow.Title = "Gemini Demo";

            Shell.StatusBar.AddItem("Hello world!", new GridLength(1, GridUnitType.Star));
            Shell.StatusBar.AddItem("Ln 44", new GridLength(100));
            Shell.StatusBar.AddItem("Col 79", new GridLength(100));

			_output.AppendLine("Started up");

		    Shell.ActiveDocumentChanged += (sender, e) => RefreshInspector();
		    RefreshInspector();
		}

        private void RefreshInspector()
        {
            if (Shell.ActiveItem != null)
                _inspectorTool.SelectedObject = new InspectableObjectBuilder()
                    .WithObjectProperties(Shell.ActiveItem, pd => pd.ComponentType == Shell.ActiveItem.GetType())
                    .ToInspectableObject();
            else
                _inspectorTool.SelectedObject = null;
        }
	}
}