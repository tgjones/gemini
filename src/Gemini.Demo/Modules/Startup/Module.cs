using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using Gemini.Framework;
using Gemini.Modules.Inspector;
using Gemini.Modules.Output;
using Gemini.Modules.PropertyGrid;
using System.Windows.Media.Imaging;
using Caliburn.Micro;

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
            MainWindow.Icon = new BitmapImage(new Uri("pack://application:,,/Resources/icon.png"));

            Shell.StatusBar.AddItem("Ready", new GridLength(1, GridUnitType.Star));
            Shell.StatusBar.AddItem("Ln 1", new GridLength(100));
            Shell.StatusBar.AddItem("Col 1", new GridLength(100));

			_output.AppendLine("Started up");

            Shell.ActiveDocumentChanged += delegate
                {
                    RefreshPropertyGrid();
                    RefreshInspector();
                };
            RefreshPropertyGrid();
		    RefreshInspector();
		}

        private void RefreshPropertyGrid()
        {
            var propertyGrid = IoC.Get<IPropertyGrid>();
            if (Shell.ActiveItem != null)
                propertyGrid.SelectedObject = Shell.ActiveItem;
            else
                propertyGrid.SelectedObject = null;
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