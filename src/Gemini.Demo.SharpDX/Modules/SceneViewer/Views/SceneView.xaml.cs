using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gemini.Demo.SharpDX.Modules.SceneViewer.ViewModels;
using Gemini.Modules.SharpDX.Controls;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using Texture2D = SharpDX.Toolkit.Graphics.Texture2D;

namespace Gemini.Demo.SharpDX.Modules.SceneViewer.Views
{
    public partial class SceneView : UserControl
    {
	    private BasicEffect _basicEffect;
	    private Texture2D _texture;

        // A yaw and pitch applied to the viewport based on input
        private System.Windows.Point _previousPosition;
        private float _yaw;
        private float _pitch;

        private GeometricPrimitive[] _geometricPrimitives;
        private int _primitiveIndex;

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
		        View = Matrix.LookAtRH(new Vector3(0, 0, 3), new Vector3(0, 0, 0), Vector3.UnitY),
		        World = Matrix.Identity,
		        PreferPerPixelLighting = true
	        };
	        _basicEffect.EnableDefaultLighting();

            _geometricPrimitives = new[]
            {
                GeometricPrimitive.Cube.New(e.GraphicsDevice),
                GeometricPrimitive.Cylinder.New(e.GraphicsDevice),
                GeometricPrimitive.GeoSphere.New(e.GraphicsDevice),
                GeometricPrimitive.Teapot.New(e.GraphicsDevice),
                GeometricPrimitive.Torus.New(e.GraphicsDevice)
            };
            _primitiveIndex = 0;

	        _texture = Texture2D.Load(e.GraphicsDevice, "Modules/SceneViewer/Resources/tile_aqua.png");
	        _basicEffect.Texture = _texture;
	        _basicEffect.TextureEnabled = true;

            _yaw = 0.5f;
            _pitch = 0.3f;
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
		        Matrix.PerspectiveFovRH((float) Math.PI / 4.0f,
			        (float) e.GraphicsDevice.BackBuffer.Width / e.GraphicsDevice.BackBuffer.Height,
			        0.1f, 100.0f);

            _geometricPrimitives[_primitiveIndex].Draw(_basicEffect);
        }

	    private void OnGraphicsControlUnloadContent(object sender, GraphicsDeviceEventArgs e)
	    {
			_texture.Dispose();
            foreach (var primitive in _geometricPrimitives)
		        primitive.Dispose();
		    _basicEffect.Dispose();
	    }

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
            _previousPosition = e.GetPosition(this);
            GraphicsControl.CaptureMouse();
            GraphicsControl.Focus();
        }

	    private void OnGraphicsControlHwndLButtonUp(object sender, MouseEventArgs e)
        {
            GraphicsControl.ReleaseMouseCapture();
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            var newIndex = _primitiveIndex + 1;
            newIndex %= _geometricPrimitives.Length;
            _primitiveIndex = newIndex;
        }
    }
}
