using Gemini.Framework.Services;

namespace Gemini.Framework
{
	public interface ITool : ILayoutItem
	{
		PaneLocation PreferredLocation { get; }
        double PreferredWidth { get; }
        double PreferredHeight { get; }

		bool IsVisible { get; set; }
	}
}