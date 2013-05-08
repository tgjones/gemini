using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Gemini.Modules.Xna.Util;

namespace Gemini.Modules.Xna.Controls
{
    public class ClippingHwndHost : HwndHost
    {
        private readonly Guid _uniqueId;
        private HwndSource _source;

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof(Visual), typeof(ClippingHwndHost),
            new PropertyMetadata(OnContentChanged));

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var hwndHost = (ClippingHwndHost) d;
            if (e.OldValue != null)
                hwndHost.RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                hwndHost.AddLogicalChild(e.NewValue);
        }

        public Visual Content
        {
            get { return (Visual) GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public ClippingHwndHost()
        {
            _uniqueId = Guid.NewGuid();
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            var param = new HwndSourceParameters("GeminiClippingHwndHost" + _uniqueId)
            {
                ParentWindow = hwndParent.Handle,
                WindowStyle = NativeMethods.WS_VISIBLE | NativeMethods.WS_CHILD
            };
            param.SetSize((int) Width, (int) Height);

            _source = new HwndSource(param) { RootVisual = Content };

            return new HandleRef(null, _source.Handle);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            _source.Dispose();
            _source = null;
        }
    }
}