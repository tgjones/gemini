using System.ComponentModel;
using System.Windows;
using System.Windows.Interactivity;
using MahApps.Metro.Controls;

namespace Gemini.Framework.Behaviors
{
    // Copied from MahApp's GlowWindowBehavior, because that one has a bug if GlowBrush is set in a style, rather than directly.
    public class CustomGlowWindowBehavior : Behavior<MetroWindow>
    {
        private GlowWindow left;
        private GlowWindow right;
        private GlowWindow top;
        private GlowWindow bottom;

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += new RoutedEventHandler(this.AssociatedObjectOnLoaded);
        }

        private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            MetroWindow metroWindow = this.AssociatedObject as MetroWindow;
            if (metroWindow != null && (metroWindow.UseNoneWindowStyle/* || metroWindow.GlowBrush == null*/))
                return;
            this.left = new GlowWindow(this.AssociatedObject, GlowDirection.Left);
            this.right = new GlowWindow(this.AssociatedObject, GlowDirection.Right);
            this.top = new GlowWindow(this.AssociatedObject, GlowDirection.Top);
            this.bottom = new GlowWindow(this.AssociatedObject, GlowDirection.Bottom);
            this.Show();
            this.Update();
            if (metroWindow == null || !metroWindow.WindowTransitionsEnabled)
            {
                this.SetOpacityTo(1.0);
            }
            else
            {
                this.StartOpacityStoryboard();
                this.AssociatedObject.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.AssociatedObjectIsVisibleChanged);
                this.AssociatedObject.Closing += (CancelEventHandler) ((o, args) =>
                {
                    if (args.Cancel)
                        return;
                    this.AssociatedObject.IsVisibleChanged -= new DependencyPropertyChangedEventHandler(this.AssociatedObjectIsVisibleChanged);
                });
            }
        }

        private void AssociatedObjectIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!this.AssociatedObject.IsVisible)
                this.SetOpacityTo(0.0);
            else
                this.StartOpacityStoryboard();
        }

        /// <summary>
        /// Updates all glow windows (visible, hidden, collapsed)
        /// 
        /// </summary>
        private void Update()
        {
            if (this.left == null || this.right == null || (this.top == null || this.bottom == null))
                return;
            this.left.Update();
            this.right.Update();
            this.top.Update();
            this.bottom.Update();
        }

        /// <summary>
        /// Sets the opacity to all glow windows
        /// 
        /// </summary>
        private void SetOpacityTo(double newOpacity)
        {
            if (this.left == null || this.right == null || (this.top == null || this.bottom == null))
                return;
            this.left.Opacity = newOpacity;
            this.right.Opacity = newOpacity;
            this.top.Opacity = newOpacity;
            this.bottom.Opacity = newOpacity;
        }

        /// <summary>
        /// Starts the opacity storyboard 0 -&gt; 1
        /// 
        /// </summary>
        private void StartOpacityStoryboard()
        {
            if (this.left == null || this.left.OpacityStoryboard == null || (this.right == null || this.right.OpacityStoryboard == null) || (this.top == null || this.top.OpacityStoryboard == null || (this.bottom == null || this.bottom.OpacityStoryboard == null)))
                return;
            this.left.BeginStoryboard(this.left.OpacityStoryboard);
            this.right.BeginStoryboard(this.right.OpacityStoryboard);
            this.top.BeginStoryboard(this.top.OpacityStoryboard);
            this.bottom.BeginStoryboard(this.bottom.OpacityStoryboard);
        }

        /// <summary>
        /// Shows all glow windows
        /// 
        /// </summary>
        private void Show()
        {
            this.left.Show();
            this.right.Show();
            this.top.Show();
            this.bottom.Show();
        }
    }
}