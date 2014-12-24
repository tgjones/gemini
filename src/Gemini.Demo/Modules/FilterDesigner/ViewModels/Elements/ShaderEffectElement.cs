using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels.Elements
{
    public abstract class ShaderEffectElement : DynamicElement
    {
        protected override void PrepareDrawingVisual(DrawingVisual drawingVisual)
        {
            drawingVisual.Effect = GetEffect();
        }

        protected override void Draw(DrawingContext drawingContext, Rect bounds)
        {
            drawingContext.DrawRectangle(
                new SolidColorBrush(Colors.Transparent), null,
                bounds);
        }

        protected abstract Effect GetEffect();
    }
}