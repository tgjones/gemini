using System.Windows;
using System.Windows.Media;
using Gemini.Modules.Toolbox;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels.Elements
{
    [ToolboxItem(typeof(GraphViewModel), "Color", "Generators", "pack://application:,,,/Modules/FilterDesigner/Resources/color_swatch.png")]
    public class ColorInput : DynamicElement
    {
        private Color _color;
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                UpdatePreviewImage();
                NotifyOfPropertyChange(() => Color);
            }
        }

        public ColorInput()
        {
            Color = Colors.Red;
            UpdatePreviewImage();
        }

        protected override void Draw(DrawingContext drawingContext, Rect bounds)
        {
            drawingContext.DrawRectangle(new SolidColorBrush(Color), null, bounds);
        }
    }
}