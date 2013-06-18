using System.Windows;
using System.Windows.Media;
using Gemini.Modules.Toolbox;

namespace Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner.ViewModels.Elements
{
    [ToolboxItem(typeof(GraphViewModel), "Color", "Generators")]
    public class ColorInput : DynamicElement
    {
        public Color Color { get; set; }

        public ColorInput()
        {
            Color = Colors.Red;
            Process();
        }

        protected override void Draw(DrawingContext drawingContext, Rect bounds)
        {
            drawingContext.DrawRectangle(new SolidColorBrush(Color), null, bounds);
        }
    }
}