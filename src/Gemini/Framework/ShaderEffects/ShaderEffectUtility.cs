using System;
using System.Windows.Media.Effects;

namespace Gemini.Framework.ShaderEffects
{
    internal static class ShaderEffectUtility
    {
        public static PixelShader GetPixelShader(string name)
        {
            return new PixelShader
            {
                UriSource = new Uri(@"pack://application:,,,/Gemini;component/Framework/ShaderEffects/" + name + ".ps")
            };
        }
    }
}