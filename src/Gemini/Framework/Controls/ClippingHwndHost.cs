using System.Collections;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Gemini.Framework.Win32;

namespace Gemini.Framework.Controls
{
    public class ClippingHwndHost : HwndHost
    {
        private HwndSource _source;

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof(Visual), typeof(ClippingHwndHost),
            new PropertyMetadata(OnContentChanged));

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var hwndHost = (ClippingHwndHost) d;

            if (e.OldValue != null)
            {
                if (hwndHost._source != null)
                    hwndHost._source.RootVisual = null;
                hwndHost.RemoveLogicalChild(e.OldValue);
            }

            if (e.NewValue != null)
            {
                hwndHost.AddLogicalChild(e.NewValue);
                if (hwndHost._source != null)
                    hwndHost._source.RootVisual = (Visual) e.NewValue;
            }
        }

        public Visual Content
        {
            get { return (Visual) GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (Content != null)
                    yield return Content;
            }
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            var param = new HwndSourceParameters("GeminiClippingHwndHost", (int) Width, (int) Height)
            {
                ParentWindow = hwndParent.Handle,
                WindowStyle = NativeMethods.WS_VISIBLE | NativeMethods.WS_CHILD,
            };

            _source = new HwndSource(param)
            {
                RootVisual = Content
            };

            return new HandleRef(null, _source.Handle);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            _source.Dispose();
            _source = null;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            // If we don't do this, HwndHost doesn't seem to pick up on all size changes.
            UpdateWindowPos();

            base.OnRenderSizeChanged(sizeInfo);
        }
    }
}