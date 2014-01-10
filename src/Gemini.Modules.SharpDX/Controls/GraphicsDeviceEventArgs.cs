#region File Description
//-----------------------------------------------------------------------------
// Copyright 2011, Nick Gravelyn.
// Licensed under the terms of the Ms-PL: 
// http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
//-----------------------------------------------------------------------------
#endregion

using System;
using SharpDX.Toolkit.Graphics;

namespace Gemini.Modules.SharpDX.Controls
{
    /// <summary>
    /// Arguments used for Device related events.
    /// </summary>
    public class GraphicsDeviceEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the GraphicsDevice.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
		/// Initializes a new GraphicsDeviceEventArgs.
        /// </summary>
		/// <param name="graphicsDevice">The GraphicsDevice associated with the event.</param>
		public GraphicsDeviceEventArgs(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }
    }
}
