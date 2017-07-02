using System.Windows.Media;
using System.Windows.Media.Imaging;
using Gemini.Modules.Toolbox;
using Gemini.Demo.Modules.FilterDesigner.ViewModels.Connector.Output;
using System.Threading.Tasks.Dataflow;
using System;
using Gridsum.DataflowEx;
using System.Threading.Tasks;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels.Elements
{
    [ToolboxItem(typeof(GraphViewModel), "Image Source", "Generators", "pack://application:,,,/Modules/FilterDesigner/Resources/image.png")]
    public class ImageSource : ElementViewModel
    {
        public Dataflow<BitmapSource, BitmapSource> source;

        private BitmapSource _bitmap;
        public BitmapSource Bitmap
        {
            get { return _bitmap; }
            set
            {
                _bitmap = value;
                NotifyOfPropertyChange(() => PreviewImage);
                _bitmap.Freeze();
                source.SendAsync(value);
            }
        }

        public override BitmapSource PreviewImage
        {
            get { return Bitmap; }
        }

        public ImageSource()
        {
            source = new DataBroadcaster<BitmapSource>(DataflowOptions.Default) { Name = this.Name };
            AddOutputConnector(new BitmapSourceOutputConnectorViewModel(this, "Output", Colors.DarkSeaGreen, source.OutputBlock));
        }
    }
}