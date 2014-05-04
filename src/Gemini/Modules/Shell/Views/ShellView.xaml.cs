using System;
using System.Collections.Generic;
using System.IO;
using Gemini.Framework;
using Gemini.Modules.Shell.ViewModels;
using Xceed.Wpf.AvalonDock.Layout;

namespace Gemini.Modules.Shell.Views
{
	public partial class ShellView : IShellView
	{
	    private LayoutRoot _layout;

	    public ShellView()
		{
			InitializeComponent();
		}

	    public void LoadLayout(Stream stream, Action<ITool> addToolCallback, Action<IDocument> addDocumentCallback,
                               Dictionary<string, ILayoutItem> itemsState)
	    {
            LayoutUtility.LoadLayout(Manager, stream, addDocumentCallback, addToolCallback, itemsState);
	    }

        public void SaveLayout(Stream stream)
        {
            LayoutUtility.SaveLayout(Manager, stream);
        }

	    private void OnManagerLayoutUpdated(object sender, EventArgs e)
	    {
	        UpdateFloatingWindows();
	    }

	    public void UpdateFloatingWindows()
	    {
            var showFloatingWindowsInTaskbar = ((ShellViewModel) DataContext).ShowFloatingWindowsInTaskbar;
            foreach (var window in Manager.FloatingWindows)
                window.ShowInTaskbar = showFloatingWindowsInTaskbar;
	    }
	}
}
