using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Gemini.Framework;

namespace Gemini.Demo.Modules.Home.ViewModels
{
	[Export(typeof(HomeViewModel))]
	public class HomeViewModel : Document
	{
	    private bool _isLeftPanelVisible = true;
        [DisplayName("Visible?")]
        public bool IsLeftPanelVisible
        {
            get { return _isLeftPanelVisible; }
            set
            {
                _isLeftPanelVisible = value;
                NotifyOfPropertyChange(() => IsLeftPanelVisible);
            }
        }

		private Color _background;
		public Color Background
		{
			get { return _background; }
			set
			{
				_background = value;
				NotifyOfPropertyChange(() => Background);
			}
		}

		private Color _foreground;
		public Color Foreground
		{
			get { return _foreground; }
			set
			{
				_foreground = value;
				NotifyOfPropertyChange(() => Foreground);
			}
		}

        private TextAlignment _textAlignment;
        [DisplayName("Text Alignment")]
        public TextAlignment TextAlignment
        {
            get { return _textAlignment; }
            set
            {
                _textAlignment = value;
                NotifyOfPropertyChange(() => TextAlignment);
            }
        }

        private Point3D _cameraPosition;
        [DisplayName("Camera Position")]
        public Point3D CameraPosition
        {
            get { return _cameraPosition; }
            set
            {
                _cameraPosition = value;
                NotifyOfPropertyChange(() => CameraPosition);
            }
        }

        private double _cameraFieldOfView;
        [DisplayName("Field of View"), Range(1.0, 180.0)]
        public double CameraFieldOfView
        {
            get { return _cameraFieldOfView; }
            set
            {
                _cameraFieldOfView = value;
                NotifyOfPropertyChange(() => CameraFieldOfView);
            }
        }

        private Point3D _lightPosition;
        [DisplayName("Light Position")]
        public Point3D LightPosition
        {
            get { return _lightPosition; }
            set
            {
                _lightPosition = value;
                NotifyOfPropertyChange(() => LightPosition);
            }
        }

		public override string DisplayName
		{
			get { return "Home"; }
		}

		public HomeViewModel()
		{
			Background = Colors.CornflowerBlue;
			Foreground = Colors.White;
		    TextAlignment = TextAlignment.Center;
		    CameraPosition = new Point3D(6, 5, 4);
		    CameraFieldOfView = 45;
		    LightPosition = new Point3D(0, 5, 0);
		}
	}
}