using System.Windows;
using System.Windows.Controls;

namespace Gemini.Framework.Controls
{
    public class ToolPaneToolBar : ToolBar
    {
        static ToolPaneToolBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolPaneToolBar),
                new FrameworkPropertyMetadata(typeof(ToolPaneToolBar)));
        }

        public ToolPaneToolBar()
        {
            SetOverflowMode(this, OverflowMode.Never);
            ToolBarTray.SetIsLocked(this, true);
        }
    }
}