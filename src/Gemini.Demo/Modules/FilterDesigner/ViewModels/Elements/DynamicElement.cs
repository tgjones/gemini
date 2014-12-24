using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels.Elements
{
    public abstract class DynamicElement : ElementViewModel
    {
        private BitmapSource _previewImage;

        public override BitmapSource PreviewImage
        {
            get { return _previewImage; }
        }

        protected DynamicElement()
        {
            SetOutputConnector("Output", Colors.DarkSeaGreen, () => PreviewImage);
        }

        protected virtual void PrepareDrawingVisual(DrawingVisual drawingVisual)
        {
            
        }

        protected abstract void Draw(DrawingContext drawingContext, Rect bounds);

        protected void UpdatePreviewImage()
        {
            var dv = new DrawingVisual();
            PrepareDrawingVisual(dv);

            DrawingContext dc = dv.RenderOpen();
            Draw(dc, new Rect(0, 0, PreviewSize, PreviewSize));
            dc.Close();

            var rtb = new RenderTargetBitmap((int) PreviewSize, (int) PreviewSize, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);

            if (dv.Effect is IDisposable)
                ((IDisposable) dv.Effect).Dispose();

            _previewImage = rtb;
            NotifyOfPropertyChange("PreviewImage");

            RaiseOutputChanged();
        }

        protected override void OnInputConnectorConnectionChanged()
        {
            UpdatePreviewImage();
        }
    }
}