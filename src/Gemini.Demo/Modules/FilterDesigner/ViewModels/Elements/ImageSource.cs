using System.Windows.Media;
using System.Windows.Media.Imaging;
using Gemini.Modules.Toolbox;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels.Elements
{
    [ToolboxItem(typeof(GraphViewModel), "Image Source", "Generators", "pack://application:,,,/Modules/FilterDesigner/Resources/image.png")]
    public class ImageSource : ElementViewModel
    {
        private BitmapSource _bitmap;
        public BitmapSource Bitmap
        {
            get { return _bitmap; }
            set
            {
                _bitmap = value;
                NotifyOfPropertyChange(() => PreviewImage);
                RaiseOutputChanged();
            }
        }

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