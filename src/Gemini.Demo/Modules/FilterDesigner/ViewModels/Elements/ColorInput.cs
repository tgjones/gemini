using System.Windows;
using System.Windows.Media;
using Gemini.Modules.Toolbox;
using System.Threading.Tasks.Dataflow;
using Gridsum.DataflowEx;
using System.Windows.Media.Imaging;
using System;
using System.Threading.Tasks;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels.Elements
{
    [ToolboxItem(typeof(GraphViewModel), "Color", "Generators", "pack://application:,,,/Modules/FilterDesigner/Resources/color_swatch.png")]
    public class ColorInput : BitmapSourceElement
    {
        private Color _color;

        public ColorInput()
        {
            Color = Colors.Red;
        }

        private BitmapSource _transform(Color arg)
        {
            var dv = new DrawingVisual();
            DrawingContext dc = dv.RenderOpen();
            dc.DrawRectangle(new SolidColorBrush(arg), null, new Rect(0, 0, PreviewSize, PreviewSize));
            dc.Close();
            return Render(dv);
        }
        internal BitmapSource Render(DrawingVisual dv)
        {
            var rtb = new RenderTargetBitmap((int)PreviewSize, (int)PreviewSize, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);

            if (dv.Effect is IDisposable)
                ((IDisposable)dv.Effect).Dispose();
            rtb.Freeze();
            return rtb;
        }
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                source.SendAsync(_transform(value));
                NotifyOfPropertyChange(() => Color);
            }
        }
    }
}