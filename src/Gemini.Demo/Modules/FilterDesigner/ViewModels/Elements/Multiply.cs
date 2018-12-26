using System.Windows.Media;
using System.Windows.Media.Effects;
using Gemini.Demo.Modules.FilterDesigner.ShaderEffects;
using Gemini.Modules.Toolbox;
using System.Windows.Media.Imaging;
using Gemini.Demo.Modules.FilterDesigner.ViewModels.Connector.Input;
using Gridsum.DataflowEx;
using System;
using System.Threading.Tasks.Dataflow;
using System.Threading.Tasks;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels.Elements
{
    [ToolboxItem(typeof(GraphViewModel), "Multiply", "Maths", "pack://application:,,,/Modules/FilterDesigner/Resources/active_x_16xLG.png")]
    public class Multiply : ShaderEffectElement
    {

        public Multiply() : base()
        {

        }

        internal override DrawingVisual GetEffect(BitmapSource Input1, BitmapSource Input2)
        {
            return new DrawingVisual
            {
                Effect = new MultiplyEffect
                {
                    Input1 = new ImageBrush(Input1),
                    Input2 = new ImageBrush(Input2)
                }
            };
        }
    }
}