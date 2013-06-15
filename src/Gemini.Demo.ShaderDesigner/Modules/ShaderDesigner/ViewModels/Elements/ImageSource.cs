using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner.ViewModels.Elements
{
    public class ImageSource : ElementViewModel
    {
        public BitmapSource Bitmap { get; set; }

        public override BitmapSource PreviewImage
        {
            get { return Bitmap; }
        }

        public ImageSource()
        {
            SetOutputConnector("Output", Colors.DarkSeaGreen);
        }
    }
}