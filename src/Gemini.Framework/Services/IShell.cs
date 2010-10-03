using System;
using Caliburn.PresentationFramework.ApplicationModel;
using Gemini.Framework.Ribbon;

namespace Gemini.Framework.Services
{
	public interface IShell
	{
		event EventHandler ActiveDocumentChanged;

		string Title { get; set; }

		IRibbon Ribbon { get; }
		IStatusBar StatusBar { get; }

		IPresenter CurrentPresenter { get; }

		void ShowTool(Pane pane, IExtendedPresenter model);

		void OpenDocument(IExtendedPresenter model);
		void CloseDocument(IExtendedPresenter document, Action<bool> completed);
		void ActivateDocument(IExtendedPresenter document);

		void Close();

		event EventHandler AfterViewLoaded;
		void OnModulesInitialized(EventArgs args);
	}
}