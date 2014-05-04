using System;
using System.Collections.Generic;
using System.IO;
using Gemini.Framework;

namespace Gemini.Modules.Shell.Views
{
    public interface IShellView
    {
        void LoadLayout(Stream stream, Action<ITool> addToolCallback, Action<IDocument> addDocumentCallback,
                        Dictionary<string, ILayoutItem> itemsState);

        void SaveLayout(Stream stream);

        void UpdateFloatingWindows();
    }
}