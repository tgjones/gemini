using System.Windows.Media;
using System.Windows.Media.Effects;
using Gemini.Demo.Modules.FilterDesigner.ShaderEffects;
using Gemini.Modules.Toolbox;
using System.Windows.Media.Imaging;
using Gemini.Demo.Modules.FilterDesigner.ViewModels.Connector.Input;
using System.Threading.Tasks.Dataflow;
using Gridsum.DataflowEx;
using System;
using System.Threading.Tasks;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels.Elements
{
    [ToolboxItem(typeof(GraphViewModel), "Add", "Maths", "pack://application:,,,/Modules/FilterDesigner/Resources/action_add_16xLG.png")]
    public class Add : ShaderEffectElement
    {
        public Add() : base()
        {

        }


        internal override DrawingVisual GetEffect(BitmapSource Input1, BitmapSource Input2)
        {
            return new DrawingVisual()
            {
                Effect = new AddEffect
                {
                    Input1 = new ImageBrush(Input1),
                    Input2 = new ImageBrush(Input2)
                }
            };
        }
    }
}