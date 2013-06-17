using System;
using System.Windows.Media.Effects;

namespace Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner.Util
{
    internal static class ShaderEffectUtility
    {
        public static PixelShader GetPixelShader(string name)
        {
            return new PixelShader
            {
                UriSource = new Uri(@"pack://application:,,,/Gemini.Demo.ShaderDesigner;component/Modules/ShaderDesigner/ShaderEffects/" + name + ".ps")
            };
        }
    }
}