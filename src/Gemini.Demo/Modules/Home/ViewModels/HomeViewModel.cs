using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Gemini.Framework;

namespace Gemini.Demo.Modules.Home.ViewModels
{
    [DisplayName("Home View Model")]
	[Export]
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

        public override bool ShouldReopenOnStart
        {
            get { return true; }
        }

        public override void SaveState(BinaryWriter writer)
        {
            // save color as byte information
            writer.Write(Background.A);
            writer.Write(Background.R);
            writer.Write(Background.G);
            writer.Write(Background.B);

            // save foreground
            writer.Write(Foreground.A);
            writer.Write(Foreground.R);
            writer.Write(Foreground.G);
            writer.Write(Foreground.B);

            // save TextAlignment as a string
            writer.Write(TextAlignment.ToString());
        }

        public override void LoadState(BinaryReader reader)
        {
            // load color
            Background = new Color { A = reader.ReadByte(), R = reader.ReadByte(), G = reader.ReadByte(), B = reader.ReadByte() };
            Foreground = new Color { A = reader.ReadByte(), R = reader.ReadByte(), G = reader.ReadByte(), B = reader.ReadByte() };

            // load TextAlignment as a string
            TextAlignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), reader.ReadString());
        }
	}
}