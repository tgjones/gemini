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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Gemini.Modules.SharpDX.Util;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Toolkit.Graphics;
using Device = SharpDX.Direct3D11.Device;
using Image = System.Windows.Controls.Image;

namespace Gemini.Modules.SharpDX.Controls
{
	public class DrawingSurface : Image
	{
		/// <summary>
		/// Occurs when the control has initialized the GraphicsDevice.
		/// </summary>
		public event EventHandler<GraphicsDeviceEventArgs> LoadContent;

		/// <summary>
		/// Occurs when the DrawingSurface has been invalidated.
		/// </summary>
		public event EventHandler<DrawEventArgs> Draw;

		/// <summary>
		/// Occurs when the control is unloading the GraphicsDevice.
		/// </summary>
		public event EventHandler<GraphicsDeviceEventArgs> UnloadContent;

		private Device _device;
		private GraphicsDevice _graphicsDevice;
		private RenderTarget2D _backBuffer;
        private D3D11ImageSource _d3DSurface;

		private bool _isRendering;
		private bool _contentNeedsRefresh;

		/// <summary>
		/// Gets or sets a value indicating whether this control will redraw every time the CompositionTarget.Rendering event is fired.
		/// Defaults to false.
		/// </summary>
		public bool AlwaysRefresh { get; set; }

		static DrawingSurface()
		{
			StretchProperty.OverrideMetadata(typeof(DrawingSurface), new FrameworkPropertyMetadata(Stretch.Fill));
		}

		public DrawingSurface()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (IsInDesignMode)
                return;

            StartD3D();
            StartRendering();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (IsInDesignMode)
                return;

            StopRendering();
            EndD3D();
        }

        private void StartD3D()
        {
            _device = new Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_11_0);
	        _graphicsDevice = GraphicsDevice.New(_device);

            _d3DSurface = new D3D11ImageSource(Window.GetWindow(this));
            _d3DSurface.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;

            CreateAndBindTargets();

            Source = _d3DSurface;

			RaiseLoadContent(new GraphicsDeviceEventArgs(_graphicsDevice));

	        _contentNeedsRefresh = true;
	        _isRendering = true;
        }

        private void EndD3D()
        {
			_isRendering = false;

			RaiseUnloadContent(new GraphicsDeviceEventArgs(_graphicsDevice));

            if (_d3DSurface != null)
                _d3DSurface.IsFrontBufferAvailableChanged -= OnIsFrontBufferAvailableChanged;
            Source = null;

            Disposer.RemoveAndDispose(ref _d3DSurface);
			Disposer.RemoveAndDispose(ref _backBuffer);
            Disposer.RemoveAndDispose(ref _device);
        }

        private void CreateAndBindTargets()
        {
	        if (_device == null)
		        return;

			_d3DSurface.SetRenderTargetDX10(null);

			Disposer.RemoveAndDispose(ref _backBuffer);

	        int width = Math.Max((int) ActualWidth, 100);
	        int height = Math.Max((int) ActualHeight, 100);

			_backBuffer = RenderTarget2D.New(_graphicsDevice, new Texture2DDescription
			{
				BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
				Format = Format.B8G8R8A8_UNorm,
				Width = width,
				Height = height,
				MipLevels = 1,
				SampleDescription = new SampleDescription(1, 0),
				Usage = ResourceUsage.Default,
				OptionFlags = ResourceOptionFlags.Shared,
				CpuAccessFlags = CpuAccessFlags.None,
				ArraySize = 1
			});

			_graphicsDevice.Presenter = new RenderTargetGraphicsPresenter(
				_graphicsDevice, _backBuffer, DepthFormat.Depth16);

            _d3DSurface.SetRenderTargetDX10(_backBuffer);
        }

        private void StartRendering()
        {
            CompositionTarget.Rendering += OnRendering;
        }

        private void StopRendering()
        {
            CompositionTarget.Rendering -= OnRendering;
        }

        private void OnRendering(object sender, EventArgs e)
        {
	        if (!_isRendering)
		        return;

	        if (_contentNeedsRefresh || AlwaysRefresh)
	        {
		        Render();
		        _d3DSurface.InvalidateD3DImage();
	        }
        }

		private void Render()
		{
			Device device = _device;
			if (device == null)
				return;

			if (_backBuffer == null)
				return;

			_graphicsDevice.SetRenderTargets(
				_graphicsDevice.DepthStencilBuffer,
				_graphicsDevice.BackBuffer);
			_graphicsDevice.SetViewport(0, 0, 
				_graphicsDevice.BackBuffer.Width, 
				_graphicsDevice.BackBuffer.Height);

			RaiseDraw(new DrawEventArgs(this, _graphicsDevice));

			_graphicsDevice.Present();
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

		protected virtual void RaiseUnloadContent(GraphicsDeviceEventArgs args)
		{
			var handler = UnloadContent;
			if (handler != null)
				handler(this, args);
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            CreateAndBindTargets();
			_contentNeedsRefresh = true;

            base.OnRenderSizeChanged(sizeInfo);
        }

        private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // this fires when the screensaver kicks in, the machine goes into sleep or hibernate
            // and any other catastrophic losses of the d3d device from WPF's point of view
	        if (_d3DSurface.IsFrontBufferAvailable)
	        {
		        CreateAndBindTargets();
		        _contentNeedsRefresh = true;
		        StartRendering();
	        }
	        else
	        {
		        StopRendering();
	        }
        }

        /// <summary>
        /// Gets a value indicating whether the control is in design mode
        /// (running in Blend or Visual Studio).
        /// </summary>
        public static bool IsInDesignMode
        {
	        get { return DesignerProperties.GetIsInDesignMode(new DependencyObject()); }
        }

		public void Invalidate()
		{
			_contentNeedsRefresh = true;
		}
	}
}