using System.Windows;
using Gemini.Demo.Xna.Primitives;
using Gemini.Framework.Controls;
using Gemini.Modules.Xna.Controls;
using Microsoft.Xna.Framework;

namespace Gemini.Demo.Xna.Modules.PrimitiveList.Controls
{
    public class RotatableCubeControl : GraphicsDeviceControl
    {
        private float _yaw, _pitch;
        private System.Windows.Point _originalPosition;

        public static readonly DependencyProperty PrimitiveProperty = DependencyProperty.Register(
            "Primitive", typeof(GeometricPrimitive), typeof(RotatableCubeControl));

        public GeometricPrimitive Primitive
        {
            get { return (GeometricPrimitive) GetValue(PrimitiveProperty); }
            set { SetValue(PrimitiveProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color", typeof(Color), typeof(RotatableCubeControl),
            new PropertyMetadata(Color.Red));

        public Color Color
        {
            get { return (Color) GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        /// <summary>
        /// Invoked after either control has created its graphics device.
        /// </summary>
        protected override void RaiseLoadContent(GraphicsDeviceEventArgs args)
        {
            // Create our 3D cube object
            Primitive.Initialize(args.GraphicsDevice);

            base.RaiseLoadContent(args);
        }

        /// <summary>
        /// Invoked when our second control is ready to render.
        /// </summary>
        protected override void RaiseRenderXna(GraphicsDeviceEventArgs args)
        {
            args.GraphicsDevice.Clear(Color.LightGreen);

            if (Primitive != null)
            {
                // Create the world-view-projection matrices for the cube and camera
                Matrix world = Matrix.CreateFromYawPitchRoll(_yaw, _pitch, 0f);
                Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 2.5f), Vector3.Zero, Vector3.Up);
                Matrix projection = Matrix.CreatePerspectiveFieldOfView(1, args.GraphicsDevice.Viewport.AspectRatio, 1, 10);

                // Draw a cube
                Primitive.Draw(world, view, projection, Color);
            }

            base.RaiseRenderXna(args);
        }

        /// <summary>
        /// Invoked when the mouse moves over the second viewport
        /// </summary>
        /// <param name="args"></param>
        protected override void RaiseHwndMouseMove(HwndMouseEventArgs args)
        {
            // If the left or right buttons are down, we adjust the yaw and pitch of the cube
            if (IsMouseCaptured)
            {
                var position = HwndMouse.GetCursorPosition();

                _yaw += (float) (position.X - _originalPosition.X) * .01f;
                _pitch += (float) (position.Y - _originalPosition.Y) * .01f;

                HwndMouse.SetCursorPosition(_originalPosition);
            }

            base.RaiseHwndMouseMove(args);
        }

        /// <summary>
        /// We use the left mouse button to do exclusive capture of the mouse so we can drag and drag
        /// to rotate the cube without ever leaving the control
        /// </summary>
        /// <param name="args"></param>
        protected override void RaiseHwndLButtonDown(HwndMouseEventArgs args)
        {
            _originalPosition = HwndMouse.GetCursorPosition();
            CaptureMouse();
            HwndMouse.HideCursor();
            base.RaiseHwndLButtonDown(args);
        }

        protected override void RaiseHwndLButtonUp(HwndMouseEventArgs args)
        {
            HwndMouse.ShowCursor();
            ReleaseMouseCapture();
            base.RaiseHwndLButtonUp(args);
        }
    }
}