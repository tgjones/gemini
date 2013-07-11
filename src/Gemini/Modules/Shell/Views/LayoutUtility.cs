using System.IO;

namespace Gemini.Modules.Shell.Views
{
    internal static class LayoutUtility
    {
        public const string LayoutFile = @".\AvalonDock.Layout.config";

        public static bool HasPersistedLayout
        {
            get { return File.Exists(LayoutFile); }
        }
    }
}