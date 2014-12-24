using System.Windows;
using System.Windows.Media;

namespace Gemini.Demo.Modules.FilterDesigner.ShaderEffects
{
	internal class AddEffect : ShaderEffectBase<AddEffect>
	{
		public static readonly DependencyProperty Input1Property = RegisterPixelShaderSamplerProperty(
            "Input1", typeof(AddEffect), 0);

		public Brush Input1
		{
		    get { return (Brush) GetValue(Input1Property); }
		    set { SetValue(Input1Property, value); }
		}

        public static readonly DependencyProperty Input2Property = RegisterPixelShaderSamplerProperty(
            "Input2", typeof(AddEffect), 1);

		public Brush Input2
		{
		    get { return (Brush) GetValue(Input2Property); }
		    set { SetValue(Input2Property, value); }
		}

		public AddEffect()
		{
			UpdateShaderValue(Input1Property);
            UpdateShaderValue(Input2Property);
		}
	}
}