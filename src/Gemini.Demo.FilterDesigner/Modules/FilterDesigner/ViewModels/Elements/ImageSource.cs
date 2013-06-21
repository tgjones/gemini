using System.Windows.Media;
using System.Windows.Media.Imaging;
using Gemini.Modules.Toolbox;

namespace Gemini.Demo.FilterDesigner.Modules.FilterDesigner.ViewModels.Elements
{
    [ToolboxItem(typeof(GraphViewModel), "Image Source", "Generators")]
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