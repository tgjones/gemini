using System.Windows.Media;
using System.Windows.Media.Effects;
using Gemini.Demo.Modules.FilterDesigner.ShaderEffects;
using Gemini.Modules.Toolbox;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels.Elements
{
    [ToolboxItem(typeof(GraphViewModel), "Multiply", "Maths", "pack://application:,,,/Modules/FilterDesigner/Resources/active_x_16xLG.png")]
    public class Multiply : ShaderEffectElement
    {
        protected override Effect GetEffect()
        {
            return new MultiplyEffect
            {
                Input1 = new ImageBrush(InputConnectors[0].Value),
                Input2 = new ImageBrush(InputConnectors[1].Value)
            };
        }

        public Multiply()
        {
            AddInputConnector("Left", Colors.DarkSeaGreen);
            AddInputConnector("Right", Colors.DarkSeaGreen);
        }
    }
}