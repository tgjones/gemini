using System.Windows.Media.Imaging;

namespace Gemini.Contracts.Gui.Controls
{
	public interface IButton : ICommandControl
	{
		string Text { get; }
		BitmapSource Icon { get; }
		bool IsCancel { get; }
		bool IsDefault { get; }
	}
}