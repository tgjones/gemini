using System;
using System.Windows.Controls;
using System.Windows.Input;
using Gemini.Demo.SharpDX.Modules.SceneViewer.ViewModels;
using Gemini.Modules.SharpDX.Controls;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Toolkit.Graphics;
using RasterizerState = SharpDX.Toolkit.Graphics.RasterizerState;
using Texture2D = SharpDX.Toolkit.Graphics.Texture2D;

namespace Gemini.Demo.SharpDX.Modules.SceneViewer.Views
{
    public partial class SceneView : UserControl
    {
	    private BasicEffect _basicEffect;
		private GeometricPrimitive _teapot;
	    private Texture2D _texture;

        // A yaw and pitch applied to the viewport based on input
        private System.Windows.Point _previousPosition;
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
	        _basicEffect = new BasicEffect(e.GraphicsDevice)
	        {
		        View = Matrix.LookAtLH(new Vector3(0, 0, 3), new Vector3(0, 0, 0), Vector3.UnitY),
		        World = Matrix.Identity,
		        PreferPerPixelLighting = true
	        };
	        _basicEffect.EnableDefaultLighting();

			_teapot = GeometricPrimitive.Teapot.New(e.GraphicsDevice);

	        _texture = Texture2D.Load(e.GraphicsDevice, "Modules/SceneViewer/Resources/GeneticaMortarlessBlocks.jpg");
	        _basicEffect.Texture = _texture;
	        _basicEffect.TextureEnabled = true;
        }

        /// <summary>
        /// Invoked when our second control is ready to render.
        /// </summary>
        private void OnGraphicsControlDraw(object sender, DrawEventArgs e)
        {
			e.GraphicsDevice.Clear(Color.CornflowerBlue);

			var position = ((SceneViewModel) DataContext).Position;
			_basicEffect.World = Matrix.RotationYawPitchRoll(_yaw, _pitch, 0f) 
				* Matrix.Translation(position);
	        _basicEffect.Projection =
		        Matrix.PerspectiveFovLH((float) Math.PI / 4.0f,
			        (float) e.GraphicsDevice.BackBuffer.Width / e.GraphicsDevice.BackBuffer.Height,
			        0.1f, 100.0f);

			_teapot.Draw(_basicEffect);
        }

        // Invoked when the mouse moves over the second viewport
	    private void OnGraphicsControlUnloadContent(object sender, GraphicsDeviceEventArgs e)
	    {
			_texture.Dispose();
		    _teapot.Dispose();
		    _basicEffect.Dispose();
	    }

	    private void OnGraphicsControlMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);

            // If the left or right buttons are down, we adjust the yaw and pitch of the cube
            if (e.LeftButton == MouseButtonState.Pressed ||
                e.RightButton == MouseButtonState.Pressed)
            {
                _yaw -= (float) (position.X - _previousPosition.X) * .01f;
                _pitch += (float) (position.Y - _previousPosition.Y) * .01f;
                GraphicsControl.Invalidate();
            }

            _previousPosition = position;
        }

	    // We use the left mouse button to do exclusive capture of the mouse so we can drag and drag

	    // to rotate the cube without ever leaving the control

	    private void OnGraphicsControlHwndLButtonDown(object sender, MouseEventArgs e)
        {
            _previousPosition = e.GetPosition(this);
            GraphicsControl.CaptureMouse();
            GraphicsControl.Focus();
        }

	    private void OnGraphicsControlHwndLButtonUp(object sender, MouseEventArgs e)
        {
            GraphicsControl.ReleaseMouseCapture();
        }
    }
}
