using System;
using Microsoft.Xna.Framework.Graphics;

namespace Gemini.Modules.Xna.Controls
{
    /// <summary>
    /// Provides data for the Draw event.
    /// </summary>
    public sealed class DrawEventArgs : EventArgs
    {
        private readonly DrawingSurface _drawingSurface;

        public GraphicsDevice GraphicsDevice
        {
            get { return _drawingSurface.GraphicsDevice; }
        }

        public DrawEventArgs(DrawingSurface drawingSurface)
        {
            _drawingSurface = drawingSurface;
        }

        public void InvalidateSurface()
        {
            _drawingSurface.Invalidate();
        }
    }
}