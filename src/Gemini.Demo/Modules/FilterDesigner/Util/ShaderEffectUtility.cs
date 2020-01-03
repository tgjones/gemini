using System;
using System.Windows.Media.Effects;

namespace Gemini.Demo.Modules.FilterDesigner.Util
{
    internal static class ShaderEffectUtility
    {
        public static PixelShader GetPixelShader(string name)
        {
            return new PixelShader
            {
                UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Modules/FilterDesigner/ShaderEffects/" + name + ".ps", UriKind.Absolute)
            };
        }
    }
}
