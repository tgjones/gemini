using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Gemini.Framework.Menus;

namespace Gemini.Framework.Services
{
	public interface IShell
	{
		WindowState WindowState { get; set; }
		string Title { get; set; }
		ImageSource Icon { get; set; }
		IMenu MainMenu { get; }
		IStatusBar StatusBar { get; }

		IScreen ActiveItem { get; }

		void ShowTool(ITool model);

		void OpenDocument(IDocument model);
		void CloseDocument(IDocument document);
		void ActivateDocument(IDocument document);

		void Close();
	}
}