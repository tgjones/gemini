using System;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Gemini.Modules.MainMenu;
using Gemini.Modules.StatusBar;
using Gemini.Modules.ToolBars;

namespace Gemini.Framework.Services
{
	public interface IShell
	{
        event EventHandler ActiveDocumentChanging;
        event EventHandler ActiveDocumentChanged;

		WindowState WindowState { get; set; }
		string Title { get; set; }
		ImageSource Icon { get; set; }

		IMenu MainMenu { get; }
        IToolBars ToolBars { get; }
		IStatusBar StatusBar { get; }

		IDocument ActiveItem { get; }

		IObservableCollection<IDocument> Documents { get; }
		IObservableCollection<ITool> Tools { get; }

		void ShowTool(ITool model);

		void OpenDocument(IDocument model);
		void CloseDocument(IDocument document);
		void ActivateDocument(IDocument document);

		void Close();
	}
}