using System;
using System.Linq;
using AvalonDock.Layout;
using Gemini.Framework.Services;

namespace Gemini.Framework.Controls
{
	public class LayoutInitializer : ILayoutUpdateStrategy
	{
		public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
		{
			if (anchorableToShow.Content is ITool)
			{
				string paneName = GetPaneName(((ITool) anchorableToShow.Content).PreferredLocation);
				var toolsPane = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(d => d.Name == paneName);
				if (toolsPane != null)
				{
					toolsPane.Children.Add(anchorableToShow);
					return true;
				}
			}

			return false;
		}

		private static string GetPaneName(PaneLocation location)
		{
			switch (location)
			{
				case PaneLocation.Left:
					return "LeftPane";
				case PaneLocation.Right:
					return "RightPane";
				case PaneLocation.Bottom:
					return "BottomPane";
				default:
					throw new ArgumentOutOfRangeException("location");
			}
		}

		public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
		{
			
		}
	}
}