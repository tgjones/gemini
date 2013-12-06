using System;
using System.Windows;
using Caliburn.Micro;
using Gemini.Modules.MainMenu;
using Gemini.Modules.StatusBar;
using Gemini.Modules.ToolBars;

namespace Gemini.Framework.Services
{
    public interface IShell : IGuardClose, IDeactivate
	{
        event EventHandler ActiveDocumentChanging;
        event EventHandler ActiveDocumentChanged;

	    event EventHandler CurrentThemeChanged;

        ResourceDictionary CurrentTheme { get; set; }

		IMenu MainMenu { get; }
        IToolBars ToolBars { get; }
		IStatusBar StatusBar { get; }

		IDocument ActiveItem { get; }

		IObservableCollection<IDocument> Documents { get; }
		IObservableCollection<ITool> Tools { get; }

		void ShowTool(ITool model);

		void OpenDocument(IDocument model);
		void CloseDocument(IDocument document);

		void Close();
	}
}