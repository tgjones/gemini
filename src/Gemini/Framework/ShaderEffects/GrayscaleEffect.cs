using System.Windows;
using System.Windows.Media;

namespace Gemini.Framework.ShaderEffects
{
    public class GrayscaleEffect : ShaderEffectBase<GrayscaleEffect>
    {
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(GrayscaleEffect), 0);

        public Brush Input
        {
            get { return (Brush) GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public GrayscaleEffect()
        {
            UpdateShaderValue(InputProperty);
        }
    }
}