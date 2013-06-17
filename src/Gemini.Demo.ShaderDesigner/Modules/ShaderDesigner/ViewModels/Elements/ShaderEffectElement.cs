using System;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner.ViewModels.Elements
{
    public abstract class ShaderEffectElement : ElementViewModel
    {
        private BitmapSource _previewImage;

        public override BitmapSource PreviewImage
        {
            get { return _previewImage; }
        }

        public void Process()
        {
            var dv = new DrawingVisual();
            var shaderEffect = GetEffect();
            dv.Effect = shaderEffect;

            DrawingContext dc = dv.RenderOpen();

            dc.DrawRectangle(new SolidColorBrush(Colors.Transparent), null, new System.Windows.Rect(0, 0, 100, 100));
            dc.Close();

            var rtb = new RenderTargetBitmap(100, 100, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);

            if (shaderEffect is IDisposable)
                ((IDisposable) shaderEffect).Dispose();

            _previewImage = rtb;
        }

        protected abstract Effect GetEffect();
    }
}