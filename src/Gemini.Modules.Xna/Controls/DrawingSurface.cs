#region Licence
//-----------------------------------------------------------------------------
// Ported from Engine Nine (http://nine.codeplex.com)

// Microsoft Public License (Ms-PL)
// 
// This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under U.S. copyright law.
// A "contribution" is the original software, or any additions or changes to the software.
// A "contributor" is any person that distributes its contribution under this license.
// "Licensed patents" are a contributor's patent claims that read directly on its contribution.
// 
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
// 
// 3. Conditions and Limitations
// (A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
// (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
// (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
// (D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
// (E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.
#endregion

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Gemini.Modules.Xna.Services;
using Gemini.Modules.Xna.Util;
using Microsoft.Xna.Framework.Graphics;

namespace Gemini.Modules.Xna.Controls
{
    /// <summary>
    /// Defines an area within which 3-D content can be composed and rendered. 
    /// </summary>
    /// <remarks>
    /// Thanks to:
    ///     maoren      (http://forums.create.msdn.com/forums/p/53048/321984.aspx#321984)
    ///     bozalina    (http://blog.bozalina.com/2010/11/xna-40-and-wpf.html)
    /// </remarks>
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

            _d3dImage.Lock();
            _d3dImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
            _d3dImage.Unlock();
        }

        private void EnsureRenderTarget()
        {
            if (_renderTarget == null)
            {
                _renderTarget = new RenderTarget2D(GraphicsDevice, (int) ActualWidth, (int) ActualHeight,
                    false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

                _d3dImage.Lock();
                var backBuffer = NativeMethods.GetRenderTargetSurface(_renderTarget);
                _d3dImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, backBuffer);
                _d3dImage.Unlock();
                Marshal.Release(backBuffer);
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

                RaiseDraw(new DrawEventArgs(this));

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