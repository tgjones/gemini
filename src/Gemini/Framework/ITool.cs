using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace Gemini.Framework
{
	public interface ITool : IScreen
	{
		ICommand CloseCommand { get; }

		PaneLocation PreferredLocation { get; }
        double PreferredWidth { get; }
        double PreferredHeight { get; }

		bool IsVisible { get; set; }
	}
}