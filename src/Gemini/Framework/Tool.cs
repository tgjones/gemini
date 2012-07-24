using Gemini.Framework.Services;

namespace Gemini.Framework
{
	public abstract class Tool : LayoutItemBase, ITool
	{
		public abstract PaneLocation PreferredLocation { get; }
	}
}