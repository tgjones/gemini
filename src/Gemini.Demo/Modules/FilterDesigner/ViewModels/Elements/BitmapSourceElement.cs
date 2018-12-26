using Gemini.Demo.Modules.FilterDesigner.ViewModels.Connector.Input;
using Gemini.Demo.Modules.FilterDesigner.ViewModels.Connector.Output;
using Gridsum.DataflowEx;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace Gemini.Demo.Modules.FilterDesigner.ViewModels.Elements
{
    public abstract class BitmapSourceElement : ElementViewModel
    {
        private BitmapSource _previewImage;

        public override BitmapSource PreviewImage
        {
            get { return _previewImage; }
        }

        public Dataflow<BitmapSource,BitmapSource> source { get; set; }

        protected BitmapSourceElement()
        {
            source = new TransformBlock<BitmapSource, BitmapSource>(X =>  displayImage(X),
                new ExecutionDataflowBlockOptions() { TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext()})
                .ToDataflow(DataflowOptions.Default, Name);
            AddOutputConnector(new BitmapSourceOutputConnectorViewModel(this, "Output", Colors.DarkSeaGreen, source.OutputBlock));
        }

        private BitmapSource displayImage(BitmapSource arg)
        {
            _previewImage = arg;
            NotifyOfPropertyChange("PreviewImage");
            return arg; 
        }
    }
}