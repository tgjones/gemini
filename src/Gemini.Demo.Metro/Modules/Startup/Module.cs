using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Modules.ErrorList;
using Gemini.Modules.Inspector;
using Gemini.Modules.Output;
using Gemini.Modules.ToolBars.Models;
using Gemini.Modules.Toolbox;
using Gemini.Modules.UndoRedo;
using Microsoft.Win32;

namespace Gemini.Demo.Metro.Modules.Startup
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(IErrorList);
                yield return typeof(IHistoryTool);
                yield return typeof(IInspectorTool);
                yield return typeof(IOutput);
                yield return typeof(IToolbox);
            }
        }

        public override void Initialize()
        {
            MainWindow.Title = "Gemini Metro Demo";

            Shell.ToolBars.Visible = true;
            Shell.ToolBars.Items.Add(new ToolBarModel
		    {
		        new ToolBarItem("Open", OpenFile).WithIcon(typeof(ModuleBase).Assembly),
                ToolBarItemBase.Separator,
                //new UndoToolBarItem(),
                //new RedoToolBarItem()
		    });

            Shell.StatusBar.AddItem("Hello world!", new GridLength(1, GridUnitType.Star));
            Shell.StatusBar.AddItem("Ln 44", new GridLength(100));
            Shell.StatusBar.AddItem("Col 79", new GridLength(100));
        }

        private IEnumerable<IResult> OpenFile()
        {
            var dialog = new OpenFileDialog();
            yield return Show.CommonDialog(dialog);
            yield return Show.Document(dialog.FileName);
        }
    }
}