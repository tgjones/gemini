using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using Gemini.Framework;

namespace Gemini.Demo.Metro.Modules.Home.ViewModels
{
    [DisplayName("Home View Model")]
	[Export(typeof(HomeViewModel))]
	public class HomeViewModel : Document
	{
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

		public HomeViewModel()
		{
		    DisplayName = "Home";
			Background = Colors.CornflowerBlue;
			Foreground = Colors.White;
		    TextAlignment = TextAlignment.Center;
		}
	}
}