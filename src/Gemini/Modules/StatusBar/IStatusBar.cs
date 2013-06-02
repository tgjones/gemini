using Caliburn.Micro;

namespace Gemini.Modules.StatusBar
{
	public interface IStatusBar
	{
        IObservableCollection<StatusBarItemViewModel> Items { get; }
	}
}