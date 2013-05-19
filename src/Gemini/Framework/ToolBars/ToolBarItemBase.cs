using Caliburn.Micro;

namespace Gemini.Framework.ToolBars
{
	public class ToolBarItemBase : PropertyChangedBase
	{
		public static ToolBarItemBase Separator
		{
			get { return new ToolBarItemSeparator(); }
		}

		public virtual string Name
		{
			get { return "-"; }
		}
	}
}