using Gridsum.DataflowEx;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System;
using Gemini.Demo.Modules.FilterDesigner.ViewModels.Connector.Input;
using Common.Logging;
using System.Threading.Tasks;
using Common.Logging.Simple;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels.Elements
{
    public abstract class ShaderEffectElement : BitmapSourceElement
    {
        public JoinAny<BitmapSource, BitmapSource> inputJoin = new JoinAny<BitmapSource, BitmapSource>();
        public Dataflow<(BitmapSource, BitmapSource), BitmapSource> shaderTransformer { get; set; }


        public ShaderEffectElement()
        {
            shaderTransformer = new TransformBlock<(BitmapSource, BitmapSource), BitmapSource>(x => UpdatePreviewImage(x.Item1, x.Item2))
                    .ToDataflow(DataflowOptions.Default, "Shader");
            AddInputConnector(new BitmapSourceInputConnectorViewModel(inputJoin.Target1.InputBlock, this, "Left", Colors.DarkSeaGreen));
            AddInputConnector(new BitmapSourceInputConnectorViewModel(inputJoin.Target2.InputBlock, this, "Right", Colors.DarkSeaGreen));
            inputJoin.source.LinkTo(shaderTransformer);
            shaderTransformer.LinkTo(source);
        }

        internal abstract DrawingVisual GetEffect(BitmapSource Input1, BitmapSource Input2);

        internal  BitmapSource UpdatePreviewImage(BitmapSource Input1, BitmapSource Input2)
        {
            var x = GetEffect(Input1, Input2);
            DrawingContext dc = x.RenderOpen();
            dc.DrawRectangle(
    new SolidColorBrush(Colors.Transparent), null,
    new Rect(0, 0, PreviewSize, PreviewSize));
            dc.Close();
            var rtb = new RenderTargetBitmap((int)PreviewSize, (int)PreviewSize, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(x);
            if (x.Effect is IDisposable)
                ((IDisposable)x.Effect).Dispose();
            rtb.Freeze();
            return (BitmapSource)rtb;
        }
    }
}
