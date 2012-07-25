using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
				var preferredLocation = ((ITool) anchorableToShow.Content).PreferredLocation;
				string paneName = GetPaneName(preferredLocation);
				var toolsPane = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(d => d.Name == paneName);
				if (toolsPane == null)
				{
					switch (preferredLocation)
					{
						case PaneLocation.Left:
						{
							var parent = layout.Descendents().OfType<LayoutPanel>().First(d => d.Orientation == Orientation.Horizontal);
							toolsPane = new LayoutAnchorablePane { DockWidth = new GridLength(200, GridUnitType.Pixel) };
							parent.InsertChildAt(0, toolsPane);
						}
							break;
						case PaneLocation.Right:
						{
							var parent = layout.Descendents().OfType<LayoutPanel>().First(d => d.Orientation == Orientation.Horizontal);
							toolsPane = new LayoutAnchorablePane { DockWidth = new GridLength(200, GridUnitType.Pixel) };
							parent.Children.Add(toolsPane);
						}
							break;
						case PaneLocation.Bottom:
						{
							var parent = layout.Descendents().OfType<LayoutPanel>().First(d => d.Orientation == Orientation.Vertical);
							toolsPane = new LayoutAnchorablePane { DockHeight = new GridLength(200, GridUnitType.Pixel) };
							parent.Children.Add(toolsPane);
						}
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				toolsPane.Children.Add(anchorableToShow);
				return true;
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