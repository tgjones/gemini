using System.Windows.Media;
using Caliburn.Micro;
using Gemini.Framework.Menus;

namespace Gemini.Framework.Services
{
	public interface IShell
	{
		string Title { get; set; }
		ImageSource Icon { get; set; }
		IMenu MainMenu { get; }
		IStatusBar StatusBar { get; }

		IScreen ActiveItem { get; }

		void ShowTool(PaneLocation pane, IScreen model);

		void OpenDocument(IScreen model);
		void CloseDocument(IScreen document);
		void ActivateDocument(IScreen document);

		void Close();
	}
}