using System;
using System.Collections.Generic;
using System.IO;
using Gemini.Framework;

namespace Gemini.Modules.Shell.Views
{
	public partial class ShellView : IShellView
    {
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
	}
}
