﻿using System;
using Caliburn.Micro;
using Gemini.Modules.MainMenu;
using Gemini.Modules.StatusBar;
using Gemini.Modules.ToolBars;
using Gemini.Modules.RecentFiles;

namespace Gemini.Framework.Services
{
    public interface IShell : IGuardClose, IDeactivate
	{
        event EventHandler ActiveDocumentChanging;
        event EventHandler ActiveDocumentChanged;

        bool ShowFloatingWindowsInTaskbar { get; set; }
        
		IMenu MainMenu { get; }
        IToolBars ToolBars { get; }
		IStatusBar StatusBar { get; }
        IRecentFiles RecentFiles { get; }

        // TODO: Rename this to ActiveItem.
        ILayoutItem ActiveLayoutItem { get; set; }

        // TODO: Rename this to SelectedDocument.
		IDocument ActiveItem { get; }

		IObservableCollection<IDocument> Documents { get; }
		IObservableCollection<ITool> Tools { get; }

        void ShowTool<TTool>() where TTool : ITool;
		void ShowTool(ITool model);

        bool TryActivateDocumentByPath(string path);
		void OpenDocument(IDocument model);
		void CloseDocument(IDocument document);

		void Close();
	}
}