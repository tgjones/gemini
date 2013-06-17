using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner.Util;

namespace Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner.ShaderEffects
{
	internal class AddEffect : ShaderEffect, IDisposable
	{
		[ThreadStatic]
		private static PixelShader _shader;

		private static PixelShader Shader
		{
			get { return (_shader ?? (_shader = ShaderEffectUtility.GetPixelShader("AddEffect"))); }
		}

		public static readonly DependencyProperty Input1Property = RegisterPixelShaderSamplerProperty("Input1", typeof(AddEffect), 0);

		public Brush Input1
		{
		    get { return (Brush) GetValue(Input1Property); }
		    set { SetValue(Input1Property, value); }
		}

        public static readonly DependencyProperty Input2Property = RegisterPixelShaderSamplerProperty("Input2", typeof(AddEffect), 1);

		public Brush Input2
		{
		    get { return (Brush) GetValue(Input2Property); }
		    set { SetValue(Input2Property, value); }
		}

		public AddEffect()
		{
			PixelShader = Shader;
			UpdateShaderValue(Input1Property);
            UpdateShaderValue(Input2Property);
		}

		void IDisposable.Dispose()
		{
			PixelShader = null;
		}
	}
}