using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Gemini.Framework.ShaderEffects
{
    /// <summary>Converts an input image into an image that blends in with the target background.</summary>
    public class ThemedImageEffect : ShaderEffectBase<ThemedImageEffect>
    {
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(ThemedImageEffect), 0);

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Color), typeof(ThemedImageEffect), new UIPropertyMetadata(Color.FromArgb(255, 246, 246, 246), PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(double), typeof(ThemedImageEffect), new UIPropertyMetadata(((double)(1D)), PixelShaderConstantCallback(1)));

        public ThemedImageEffect()
        {
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(BackgroundProperty);
            UpdateShaderValue(IsEnabledProperty);
        }

        public Brush Input
        {
            get { return ((Brush)(GetValue(InputProperty))); }
            set { SetValue(InputProperty, value); }
        }

        /// <summary>Background color of the image.</summary>
        public Color Background
        {
            get { return ((Color)(GetValue(BackgroundProperty))); }
            set { SetValue(BackgroundProperty, value); }
        }

        /// <summary>1.0 if the image should be rendered enabled, 0.0 if it should be disabled (grayscaled).</summary>
        public double IsEnabled
        {
            get { return ((double)(this.GetValue(IsEnabledProperty))); }
            set { this.SetValue(IsEnabledProperty, value); }
        }
    }
}
