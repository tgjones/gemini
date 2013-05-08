using System.Windows.Controls;
using Gemini.Demo.Xna.Modules.SceneViewer.Primitives;
using Gemini.Modules.Xna.Controls;
using Microsoft.Xna.Framework;

namespace Gemini.Demo.Xna.Modules.SceneViewer.Views
{
    /// <summary>
    /// Interaction logic for SceneView.xaml
    /// </summary>
    public partial class SceneView : UserControl
    {
        private CubePrimitive _cube;

        // A yaw and pitch applied to the viewport based on input
        private float _yaw;
        private float _pitch;

        public SceneView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked after either control has created its graphics device.
        /// </summary>
        private void OnGraphicsControlLoadContent(object sender, GraphicsDeviceEventArgs e)
        {
            // Create our 3D cube object
            _cube = new CubePrimitive(e.GraphicsDevice);
        }

        /// <summary>
        /// Invoked when our second control is ready to render.
        /// </summary>
        private void OnGraphicsControlRenderXna(object sender, GraphicsDeviceEventArgs e)
        {
            e.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Create the world-view-projection matrices for the cube and camera
            Matrix world = Matrix.CreateFromYawPitchRoll(_yaw, _pitch, 0f);
            Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 2.5f), Vector3.Zero, Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(1, e.GraphicsDevice.Viewport.AspectRatio, 1, 10);

            // Draw a cube
            _cube.Draw(world, view, projection, Color.Red);
        }

        // Invoked when the mouse moves over the second viewport
        private void OnGraphicsControlMouseMove(object sender, HwndMouseEventArgs e)
        {
            // If the left or right buttons are down, we adjust the yaw and pitch of the cube
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed ||
                e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                _yaw += (float) (e.Position.X - e.PreviousPosition.X) * .01f;
                _pitch += (float) (e.Position.Y - e.PreviousPosition.Y) * .01f;
            }
        }

        // We use the left mouse button to do exclusive capture of the mouse so we can drag and drag
        // to rotate the cube without ever leaving the control
        private void OnGraphicsControlHwndLButtonDown(object sender, HwndMouseEventArgs e)
        {
            GraphicsControl.CaptureMouse();
        }

        private void OnGraphicsControlHwndLButtonUp(object sender, HwndMouseEventArgs e)
        {
            GraphicsControl.ReleaseMouseCapture();
        }
    }
}
