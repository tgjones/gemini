#region File Description
//-----------------------------------------------------------------------------
// Copyright 2011, Nick Gravelyn.
// Licensed under the terms of the Ms-PL: 
// http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;

namespace Gemini.Modules.Xna.Controls
{
    /// <summary>
    /// Helper class responsible for creating and managing the GraphicsDevice.
    /// All GraphicsDeviceControl instances share the same GraphicsDeviceService,
    /// so even though there can be many controls, there will only ever be a single
    /// underlying GraphicsDevice. This implements the standard IGraphicsDeviceService
    /// interface, which provides notification events for when the device is reset
    /// or disposed.
    /// </summary>
    class GraphicsDeviceService : IGraphicsDeviceService
    {
        // Singleton device service instance.
        private static readonly GraphicsDeviceService SingletonInstance = new GraphicsDeviceService();

        // Keep track of how many controls are sharing the singletonInstance.
        private static int _referenceCount;

        private GraphicsDevice _graphicsDevice;

        // Store the current device settings.
        private PresentationParameters _parameters;

        /// <summary>
        /// Gets the current graphics device.
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get { return _graphicsDevice; }
        }


        // IGraphicsDeviceService events.
        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;

        /// <summary>
        /// Constructor is private, because this is a singleton class:
        /// client controls should use the public AddRef method instead.
        /// </summary>
        private GraphicsDeviceService() { }

        private void CreateDevice(IntPtr windowHandle, int width, int height)
        {
            _parameters = new PresentationParameters();

            _parameters.BackBufferWidth = Math.Max(width, 1);
            _parameters.BackBufferHeight = Math.Max(height, 1);
            _parameters.BackBufferFormat = SurfaceFormat.Color;
            _parameters.DepthStencilFormat = DepthFormat.Depth24;
            _parameters.DeviceWindowHandle = windowHandle;
            _parameters.PresentationInterval = PresentInterval.Immediate;
            _parameters.IsFullScreen = false;

            _graphicsDevice = new GraphicsDevice(
                GraphicsAdapter.DefaultAdapter,
                GraphicsProfile.Reach,
                _parameters);

            if (DeviceCreated != null)
                DeviceCreated(this, EventArgs.Empty);
        }


        /// <summary>
        /// Gets a reference to the singleton instance.
        /// </summary>
        public static GraphicsDeviceService AddRef(IntPtr windowHandle, int width, int height)
        {
            // Increment the "how many controls sharing the device" reference count.
            if (Interlocked.Increment(ref _referenceCount) == 1)
            {
                // If this is the first control to start using the
                // device, we must create the device.
                SingletonInstance.CreateDevice(windowHandle, width, height);
            }

            return SingletonInstance;
        }


        /// <summary>
        /// Releases a reference to the singleton instance.
        /// </summary>
        public void Release(bool disposing)
        {
            // Decrement the "how many controls sharing the device" reference count.
            if (Interlocked.Decrement(ref _referenceCount) == 0)
            {
                // If this is the last control to finish using the
                // device, we should dispose the singleton instance.
                if (disposing)
                {
                    if (DeviceDisposing != null)
                        DeviceDisposing(this, EventArgs.Empty);

                    _graphicsDevice.Dispose();
                }

                _graphicsDevice = null;
            }
        }

        
        /// <summary>
        /// Resets the graphics device to whichever is bigger out of the specified
        /// resolution or its current size. This behavior means the device will
        /// demand-grow to the largest of all its GraphicsDeviceControl clients.
        /// </summary>
        public void ResetDevice(int width, int height)
        {
            if (DeviceResetting != null)
                DeviceResetting(this, EventArgs.Empty);

            _parameters.BackBufferWidth = Math.Max(_parameters.BackBufferWidth, width);
            _parameters.BackBufferHeight = Math.Max(_parameters.BackBufferHeight, height);

            _graphicsDevice.Reset(_parameters);

            if (DeviceReset != null)
                DeviceReset(this, EventArgs.Empty);
        }
    }
}
