using System;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Interop;
using Gemini.Framework.Win32;

namespace Gemini.Framework.Behaviors
{
    public class WindowOptionsBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register(
            "ShowIcon", typeof(bool), typeof(WindowOptionsBehavior), 
            new PropertyMetadata(true, OnWindowOptionChanged));

        public bool ShowIcon
        {
            get { return (bool) GetValue(ShowIconProperty); }
            set { SetValue(ShowIconProperty, value); }
        }

        public static readonly DependencyProperty ShowMinimizeBoxProperty = DependencyProperty.Register(
            "ShowMinimizeBox", typeof(bool), typeof(WindowOptionsBehavior),
            new PropertyMetadata(true, OnWindowOptionChanged));

        public bool ShowMinimizeBox
        {
            get { return (bool) GetValue(ShowMinimizeBoxProperty); }
            set { SetValue(ShowMinimizeBoxProperty, value); }
        }

        public static readonly DependencyProperty ShowMaximizeBoxProperty = DependencyProperty.Register(
            "ShowMaximizeBox", typeof(bool), typeof(WindowOptionsBehavior),
            new PropertyMetadata(true, OnWindowOptionChanged));

        public bool ShowMaximizeBox
        {
            get { return (bool) GetValue(ShowMaximizeBoxProperty); }
            set { SetValue(ShowMaximizeBoxProperty, value); }
        }

        private static void OnWindowOptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WindowOptionsBehavior) d).UpdateWindowStyle();
        }

        private void UpdateWindowStyle()
        {
            if (AssociatedObject == null)
                return;

            var handle = new WindowInteropHelper(AssociatedObject).Handle;

            var windowStyle = NativeMethods.GetWindowLong(handle, NativeMethods.GWL_STYLE);

            if (ShowMinimizeBox)
                windowStyle |= NativeMethods.WS_MINIMIZEBOX;
            else
                windowStyle &= ~NativeMethods.WS_MINIMIZEBOX;

            if (ShowMaximizeBox)
                windowStyle |= NativeMethods.WS_MAXIMIZEBOX;
            else
                windowStyle &= ~NativeMethods.WS_MAXIMIZEBOX;

            NativeMethods.SetWindowLong(handle, NativeMethods.GWL_STYLE, windowStyle);

            if (ShowIcon)
            {
                // TODO
            }
            else
            {
                var exWindowStyle = NativeMethods.GetWindowLong(handle, NativeMethods.GWL_EXSTYLE);
                NativeMethods.SetWindowLong(handle, NativeMethods.GWL_EXSTYLE,
                    exWindowStyle | NativeMethods.WS_EX_DLGMODALFRAME);

                NativeMethods.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0,
                    NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOZORDER | NativeMethods.SWP_FRAMECHANGED);

                NativeMethods.SendMessage(handle, NativeMethods.WM_SETICON, IntPtr.Zero, IntPtr.Zero);
            }
        }

        protected override void OnAttached()
        {
            AssociatedObject.SourceInitialized += OnSourceInitialized;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SourceInitialized -= OnSourceInitialized;
            base.OnDetaching();
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            UpdateWindowStyle();
        }
    }
}