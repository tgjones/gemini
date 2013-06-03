using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Gemini.Framework;
using Gemini.Framework.Services;
using Xceed.Wpf.AvalonDock.Layout;

namespace Gemini.Modules.Shell.Controls
{
	public class LayoutInitializer : ILayoutUpdateStrategy
	{
		public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
		{
		    var tool = anchorableToShow.Content as ITool;
		    if (tool != null)
			{
				var preferredLocation = tool.PreferredLocation;
				string paneName = GetPaneName(preferredLocation);
				var toolsPane = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(d => d.Name == paneName);
				if (toolsPane == null)
				{
                    switch (preferredLocation)
                    {
                        case PaneLocation.Left:
                            {
                                var parent = layout.Descendents().OfType<LayoutPanel>().First(d => d.Orientation == Orientation.Horizontal);
                                toolsPane = new LayoutAnchorablePane { DockWidth = new GridLength(tool.PreferredWidth, GridUnitType.Pixel), Name = paneName };
                                parent.InsertChildAt(0, toolsPane);
                            }
                            break;
                        case PaneLocation.Right:
                            {
                                var parent = layout.Descendents().OfType<LayoutPanel>().First(d => d.Orientation == Orientation.Horizontal);
                                toolsPane = new LayoutAnchorablePane { DockWidth = new GridLength(tool.PreferredWidth, GridUnitType.Pixel), Name = paneName };
                                parent.Children.Add(toolsPane);
                            }
                            break;
                        case PaneLocation.Bottom:
                            {
                                var parent = layout.Descendents().OfType<LayoutPanel>().First(d => d.Orientation == Orientation.Vertical);
                                toolsPane = new LayoutAnchorablePane { DockHeight = new GridLength(tool.PreferredHeight, GridUnitType.Pixel), Name = paneName };
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

	    public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
	    {
            return false;
	    }

	    public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
	    {
	        
	    }
	}
}