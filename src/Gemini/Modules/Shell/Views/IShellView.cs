using System;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace Gemini.Modules.Shell.Views
{
	public interface IShellView
	{
		event EventHandler ActiveDocumentChanged;

		void ShowTool(PaneLocation pane, IScreen model);
		void OpenDocument(IScreen model);
	}
}