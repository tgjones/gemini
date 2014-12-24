using System.IO;
using System.Windows.Media.Imaging;

namespace Gemini.Demo.Modules.FilterDesigner.Util
{
    internal static class BitmapUtility
    {
        public static BitmapSource CreateFromBytes(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                var result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();

                return result;
            }
        }
    }
}