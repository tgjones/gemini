using System.Windows.Media;
using System.Windows.Media.Imaging;
using Gemini.Modules.Toolbox;

namespace Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner.ViewModels.Elements
{
    [ToolboxItem(typeof(GraphViewModel), "Image Source", "Generators")]
    public class ImageSource : ElementViewModel
    {
        public BitmapSource Bitmap { get; set; }

        public override BitmapSource PreviewImage
        {
            get { return Bitmap; }
        }

        public ImageSource()
        {
            SetOutputConnector("Output", Colors.DarkSeaGreen, () => Bitmap);
        }
    }
}