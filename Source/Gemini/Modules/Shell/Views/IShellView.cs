using Caliburn.PresentationFramework.ApplicationModel;
using Gemini.Framework.Ribbon;
using Gemini.Framework.Services;

namespace Gemini.Modules.Shell.Views
{
	public interface IShellView
	{
		void InitializeRibbon(IRibbon ribbonModel);
		void ShowTool(Pane pane, IExtendedPresenter model);
		void OpenDocument(IExtendedPresenter model);
	}
}