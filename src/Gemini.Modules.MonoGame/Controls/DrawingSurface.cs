// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Gemini.Modules.MonoGame.Services;
using Microsoft.Xna.Framework.Graphics;

namespace Gemini.Modules.MonoGame.Controls
{
	public class DrawingSurface : ContentControl, IDisposable
	{
		/// <summary>
		/// Occurs when the control has initialized the GraphicsDevice.
		/// </summary>
		public event EventHandler<GraphicsDeviceEventArgs> LoadContent;

		/// <summary>
		/// Occurs when the DrawingSurface has been invalidated.
		/// </summary>
		public event EventHandler<DrawEventArgs> Draw;

	    private GraphicsDeviceService _graphicsDeviceService;
	    private readonly D3DImage _d3dImage;
	    private readonly Image _image;
		private RenderTarget2D _renderTarget;
	    private SharpDX.Direct3D9.Texture _renderTargetD3D9;

		private bool _contentNeedsRefresh;

		/// <summary>
		/// Gets or sets a value indicating whether this control will redraw every time the CompositionTarget.Rendering event is fired.
		/// Defaults to false.
		/// </summary>
		public bool AlwaysRefresh { get; set; }

        public GraphicsDevice GraphicsDevice
        {
            get { return _graphicsDeviceService.GraphicsDevice; }
        }

        public DrawingSurface()
        {
            _d3dImage = new D3DImage();

            _image = new Image { Source = _d3dImage, Stretch = Stretch.None };
            AddChild(_image);

            _d3dImage.IsFrontBufferAvailableChanged += OnD3DImageIsFrontBufferAvailableChanged;

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            RemoveBackBufferReference();
            _contentNeedsRefresh = true;

            base.OnRenderSizeChanged(sizeInfo);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_graphicsDeviceService == null)
            {
                DeviceService.StartD3D(Window.GetWindow(this));

                // We use a render target, so the back buffer dimensions don't matter.
                _graphicsDeviceService = GraphicsDeviceService.AddRef(1, 1);
                _graphicsDeviceService.DeviceResetting += OnGraphicsDeviceServiceDeviceResetting;

                // Invoke the LoadContent event
                RaiseLoadContent(new GraphicsDeviceEventArgs(_graphicsDeviceService.GraphicsDevice));

                EnsureRenderTarget();

                CompositionTarget.Rendering += OnCompositionTargetRendering;

                _contentNeedsRefresh = true;
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_graphicsDeviceService != null)
            {
                RemoveBackBufferReference();

                CompositionTarget.Rendering -= OnCompositionTargetRendering;

                _graphicsDeviceService.DeviceResetting -= OnGraphicsDeviceServiceDeviceResetting;
                _graphicsDeviceService = null;

                DeviceService.EndD3D();
            }
        }

        private void OnGraphicsDeviceServiceDeviceResetting(object sender, EventArgs e)
        {
            RemoveBackBufferReference();
            _contentNeedsRefresh = true;
        }

        /// <summary>
        /// If we didn't do this, D3DImage would keep an reference to the backbuffer that causes the device reset below to fail.
        /// </summary>
        private void RemoveBackBufferReference()
        {
            if (_renderTarget != null)
            {
                _renderTarget.Dispose();
                _renderTarget = null;
            }

            if (_renderTargetD3D9 != null)
            {
                _renderTargetD3D9.Dispose();
                _renderTargetD3D9 = null;
            }

            _d3dImage.Lock();
            _d3dImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
            _d3dImage.Unlock();
        }

        private void EnsureRenderTarget()
        {
            if (_renderTarget == null)
            {
                _renderTarget = new RenderTarget2D(GraphicsDevice, (int) ActualWidth, (int) ActualHeight,
                    false, SurfaceFormat.Bgra32, DepthFormat.Depth24Stencil8, 1, 
                    RenderTargetUsage.PlatformContents, true);

                var handle = _renderTarget.GetSharedHandle();
                if (handle == IntPtr.Zero)
                    throw new ArgumentException("Handle could not be retrieved");

                _renderTargetD3D9 = new SharpDX.Direct3D9.Texture(DeviceService.D3DDevice,
                    _renderTarget.Width, _renderTarget.Height,
                    1, SharpDX.Direct3D9.Usage.RenderTarget, SharpDX.Direct3D9.Format.A8R8G8B8,
                    SharpDX.Direct3D9.Pool.Default, ref handle);

                using (SharpDX.Direct3D9.Surface surface = _renderTargetD3D9.GetSurfaceLevel(0))
                {
                    _d3dImage.Lock();
                    _d3dImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.NativePointer);
                    _d3dImage.Unlock();
                }
            }
        }

        private void OnD3DImageIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_d3dImage.IsFrontBufferAvailable)
                _contentNeedsRefresh = true;
        }

        private void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            if ((_contentNeedsRefresh || AlwaysRefresh) && BeginDraw())
            {
                _contentNeedsRefresh = false;

                _d3dImage.Lock();

                EnsureRenderTarget();
                GraphicsDevice.SetRenderTarget(_renderTarget);

                SetViewport();

                RaiseDraw(new DrawEventArgs(this, _graphicsDeviceService.GraphicsDevice));

                _graphicsDeviceService.GraphicsDevice.Flush();

                _d3dImage.AddDirtyRect(new Int32Rect(0, 0, (int) ActualWidth, (int) ActualHeight));

                _d3dImage.Unlock();

                GraphicsDevice.SetRenderTarget(null);
            }
        }

        protected virtual void RaiseLoadContent(GraphicsDeviceEventArgs args)
        {
            var handler = LoadContent;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void RaiseDraw(DrawEventArgs args)
        {
            var handler = Draw;
            if (handler != null)
                handler(this, args);
        }

        private bool BeginDraw()
        {
            // If we have no graphics device, we must be running in the designer.
            if (_graphicsDeviceService == null)
                return false;

            if (!_d3dImage.IsFrontBufferAvailable)
                return false;

            // Make sure the graphics device is big enough, and is not lost.
            if (!HandleDeviceReset())
                return false;

            return true;
        }

        private void SetViewport()
        {
            // Many GraphicsDeviceControl instances can be sharing the same
            // GraphicsDevice. The device backbuffer will be resized to fit the
            // largest of these controls. But what if we are currently drawing
            // a smaller control? To avoid unwanted stretching, we set the
            // viewport to only use the top left portion of the full backbuffer.
            _graphicsDeviceService.GraphicsDevice.Viewport = new Viewport(
                0, 0, Math.Max(1, (int) ActualWidth), Math.Max(1, (int) ActualHeight));
        }

        private bool HandleDeviceReset()
        {
            bool deviceNeedsReset = false;

            switch (_graphicsDeviceService.GraphicsDevice.GraphicsDeviceStatus)
            {
                case GraphicsDeviceStatus.Lost:
                    // If the graphics device is lost, we cannot use it at all.
                    return false;

                case GraphicsDeviceStatus.NotReset:
                    // If device is in the not-reset state, we should try to reset it.
                    deviceNeedsReset = true;
                    break;
            }

            if (deviceNeedsReset)
            {
                Debug.WriteLine("Resetting Device");
                _graphicsDeviceService.ResetDevice((int) ActualWidth, (int) ActualHeight);
                return false;
            }

            return true;
        }

        public void Invalidate()
        {
            _contentNeedsRefresh = true;
        }

        #region IDisposable

        private bool _isDisposed;

        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (_renderTarget != null)
                    _renderTarget.Dispose();
                if (_renderTargetD3D9 != null)
                    _renderTargetD3D9.Dispose();
                if (_graphicsDeviceService != null)
                    _graphicsDeviceService.Release(disposing);
                _isDisposed = true;
            }
        }

        ~DrawingSurface()
        {
            Dispose(false);
        }

        #endregion
	}
}