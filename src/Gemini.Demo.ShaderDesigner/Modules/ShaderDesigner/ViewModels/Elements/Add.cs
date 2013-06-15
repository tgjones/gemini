using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner.ViewModels.Elements
{
    public class Add : ElementViewModel
    {
        public override BitmapSource PreviewImage
        {
            get { return new BitmapImage(); }
        }

        public Add()
        {
            AddInputConnector("Left", Colors.DarkSeaGreen);
            AddInputConnector("Right", Colors.DarkSeaGreen);

            SetOutputConnector("Output", Colors.DarkSeaGreen);
        }
    }
}