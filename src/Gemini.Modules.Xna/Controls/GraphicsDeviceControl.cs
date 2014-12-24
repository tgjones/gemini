#region File Description
//-----------------------------------------------------------------------------
// Copyright 2011, Nick Gravelyn.
// Licensed under the terms of the Ms-PL: 
// http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Windows;
using Gemini.Framework.Controls;
using Gemini.Modules.Xna.Services;
using Microsoft.Xna.Framework.Graphics;

namespace Gemini.Modules.Xna.Controls
{
    /// <summary>
    /// A control that enables XNA graphics rendering inside a WPF control through
    /// the use of a hosted child Hwnd.
    /// </summary>
    public class GraphicsDeviceControl : HwndWrapper
    {
        // The GraphicsDeviceService that provides and manages our GraphicsDevice
        private GraphicsDeviceService _graphicsService;

        /// <summary>
        /// Invoked when the control has initialized the GraphicsDevice.
        /// </summary>
        public event EventHandler<GraphicsDeviceEventArgs> LoadContent;

        /// <summary>
        /// Invoked when the control is ready to render XNA content
        /// </summary>
        public event EventHandler<GraphicsDeviceEventArgs> RenderXna;

        #region Construction and Disposal

        public GraphicsDeviceControl()
        {
            // We must be notified of the control finishing loading so we can get the GraphicsDeviceService
            Loaded += OnXnaWindowHostLoaded;

            // We must be notified of the control changing sizes so we can resize the GraphicsDeviceService
            SizeChanged += OnXnaWindowHostSizeChanged;
        }

        protected override void Dispose(bool disposing)
        {
            // Release our reference to the GraphicsDeviceService if we have one
            if (_graphicsService != null)
            {
                _graphicsService.Release(disposing);
                _graphicsService = null;
            }

            SizeChanged -= OnXnaWindowHostSizeChanged;
            Loaded -= OnXnaWindowHostLoaded;

            base.Dispose(disposing);
        }

        #endregion

        protected override void Render(IntPtr windowHandle)
        {
            // If we have no graphics service, we can't draw
            if (_graphicsService == null)
                return;

            // Get the current width and height of the control
            var width = (int) ActualWidth;
            var height = (int) ActualHeight;

            // This piece of code is copied from the WinForms equivalent
            var deviceResetStatus = HandleDeviceReset(width, height);
            if (deviceResetStatus != GraphicsDeviceResetStatus.Normal)
                return;

            // Create the active viewport to which we'll render our content
            var viewport = new Viewport(0, 0, width, height);
            _graphicsService.GraphicsDevice.Viewport = viewport;

            // Invoke the event to render this control
            RaiseRenderXna(new GraphicsDeviceEventArgs(_graphicsService.GraphicsDevice));

            // Present to the screen, but only use the visible area of the back buffer
            _graphicsService.GraphicsDevice.Present(viewport.Bounds, null, windowHandle);
            //_graphicsService.GraphicsDevice.Present();
        }

        /// <summary>
        /// Helper used by <see cref="Render"/>. 
        /// This checks the graphics device status,
        /// making sure it is big enough for drawing the current control, and
        /// that the device is not lost. Returns an error string if the device
        /// could not be reset.
        /// </summary>
        private GraphicsDeviceResetStatus HandleDeviceReset(int width, int height)
        {
            bool deviceNeedsReset;

            switch (_graphicsService.GraphicsDevice.GraphicsDeviceStatus)
            {
                case GraphicsDeviceStatus.Lost:
                    // If the graphics device is lost, we cannot use it at all.
                    return GraphicsDeviceResetStatus.Lost;

                case GraphicsDeviceStatus.NotReset:
                    // If device is in the not-reset state, we should try to reset it.
                    deviceNeedsReset = true;
                    break;

                default:
                    // If the device state is ok, check whether it is big enough.
                    PresentationParameters pp = _graphicsService.GraphicsDevice.PresentationParameters;

                    deviceNeedsReset = (width > pp.BackBufferWidth) ||
                                       (height > pp.BackBufferHeight);
                    break;
            }

            // Do we need to reset the device?
            if (deviceNeedsReset)
            {
                try
                {
                    _graphicsService.ResetDevice(width, height);
                }
                catch
                {
                    return GraphicsDeviceResetStatus.ResetFailed;
                }
            }

            return GraphicsDeviceResetStatus.Normal;
        }

        private enum GraphicsDeviceResetStatus
        {
            Normal,
            Lost,
            ResetFailed
        }

        private void OnXnaWindowHostLoaded(object sender, RoutedEventArgs e)
        {
            // If we don't yet have a GraphicsDeviceService reference, we must add one for this control
            if (_graphicsService == null)
            {
                _graphicsService = GraphicsDeviceService.AddRef((int) ActualWidth, (int) ActualHeight);

                // Invoke the LoadContent event
                RaiseLoadContent(new GraphicsDeviceEventArgs(_graphicsService.GraphicsDevice));
            }
        }

        protected virtual void RaiseLoadContent(GraphicsDeviceEventArgs args)
        {
            var handler = LoadContent;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void RaiseRenderXna(GraphicsDeviceEventArgs args)
        {
            var handler = RenderXna;
            if (handler != null)
                handler(this, args);
        }

        private void OnXnaWindowHostSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // If we have a reference to the GraphicsDeviceService, we must reset it based on our updated size
            if (_graphicsService != null)
                _graphicsService.ResetDevice((int) ActualWidth, (int) ActualHeight);
        }
    }
}