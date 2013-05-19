using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Gemini.Framework.Menus;
using Gemini.Framework.ToolBars;

namespace Gemini.Framework.Services
{
	public interface IShell
	{
		WindowState WindowState { get; set; }
		string Title { get; set; }
		ImageSource Icon { get; set; }

		IMenu MainMenu { get; }

        bool IsToolBarVisible { get; set; }
        IToolBar ToolBar { get; }

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