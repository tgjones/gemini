using Caliburn.Micro;
using Gemini.Modules.StatusBar.ViewModels;

namespace Gemini.Modules.StatusBar
{
	public interface IStatusBar
	{
        IObservableCollection<StatusBarItemViewModel> Items { get; }
	}
}