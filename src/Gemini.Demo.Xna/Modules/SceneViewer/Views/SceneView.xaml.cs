using System;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Demo.Xna.Modules.SceneViewer.ViewModels;
using Gemini.Demo.Xna.Primitives;
using Gemini.Modules.Output;
using Gemini.Modules.Xna.Controls;
using Microsoft.Xna.Framework;

namespace Gemini.Demo.Xna.Modules.SceneViewer.Views
{
    /// <summary>
    /// Interaction logic for SceneView.xaml
    /// </summary>
    public partial class SceneView : UserControl, IDisposable
    {
        private readonly IOutput _output;
        private readonly CubePrimitive _cube;

        // A yaw and pitch applied to the viewport based on input
        private System.Windows.Point _previousPosition;
        private float _yaw;
        private float _pitch;

        public SceneView()
        {
            InitializeComponent();
            _output = IoC.Get<IOutput>();
            _cube = new CubePrimitive();
        }

        public void Dispose()
        {
            GraphicsControl.Dispose();
        }

        /// <summary>
        /// Invoked after either control has created its graphics device.
        /// </summary>
        private void OnGraphicsControlLoadContent(object sender, GraphicsDeviceEventArgs e)
        {
            // Create our 3D cube object
            _cube.Initialize(e.GraphicsDevice);
        }

        /// <summary>
        /// Invoked when our second control is ready to render.
        /// </summary>
        private void OnGraphicsControlDraw(object sender, DrawEventArgs e)
        {
            e.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Create the world-view-projection matrices for the cube and camera
            var position = ((SceneViewModel) DataContext).Position;
            Matrix world = Matrix.CreateFromYawPitchRoll(_yaw, _pitch, 0f) * Matrix.CreateTranslation(position);
            Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 2.5f), Vector3.Zero, Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(1, e.GraphicsDevice.Viewport.AspectRatio, 1, 10);

            // Draw a cube
            _cube.Draw(world, view, projection, Color.Red);
        }

        // Invoked when the mouse moves over the second viewport
        private void OnGraphicsControlMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);

            // If the left or right buttons are down, we adjust the yaw and pitch of the cube
            if (e.LeftButton == MouseButtonState.Pressed ||
                e.RightButton == MouseButtonState.Pressed)
            {
                _yaw += (float) (position.X - _previousPosition.X) * .01f;
                _pitch += (float) (position.Y - _previousPosition.Y) * .01f;
                GraphicsControl.Invalidate();
            }

            _previousPosition = position;
        }

        // We use the left mouse button to do exclusive capture of the mouse so we can drag and drag
        // to rotate the cube without ever leaving the control
        private void OnGraphicsControlHwndLButtonDown(object sender, MouseEventArgs e)
        {
            _output.AppendLine("Mouse left button down");
            _previousPosition = e.GetPosition(this);
            GraphicsControl.CaptureMouse();
            GraphicsControl.Focus();
        }

        private void OnGraphicsControlHwndLButtonUp(object sender, MouseEventArgs e)
        {
            _output.AppendLine("Mouse left button up");
            GraphicsControl.ReleaseMouseCapture();
        }

        private void OnGraphicsControlKeyDown(object sender, KeyEventArgs e)
        {
            _output.AppendLine("Key down: " + e.Key);
        }

        private void OnGraphicsControlKeyUp(object sender, KeyEventArgs e)
        {
            _output.AppendLine("Key up: " + e.Key);
        }

        private void OnGraphicsControlHwndMouseWheel(object sender, MouseWheelEventArgs e)
        {
            _output.AppendLine("Mouse wheel: " + e.Delta);
        }
    }
}
