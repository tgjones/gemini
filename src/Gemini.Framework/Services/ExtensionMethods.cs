using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Practices.ServiceLocation;
using Caliburn.PresentationFramework.ApplicationModel;

namespace Gemini.Framework.Services
{
	public static class ExtensionMethods
    {
        public static void SetShortcut(this IInputManager manager, InputGesture gesture, object handler)
        {
            manager.SetShortcut(Application.Current.MainWindow, gesture, handler);
        }

        public static IExtendedPresenter GetEditor(this IServiceLocator serviceLocator, string path)
        {
            foreach(var provider in serviceLocator.GetAllInstances<IEditorProvider>())
            {
                if (provider.Handles(path))
                    return provider.Create(path);
            }

            return null;
        }

        public static string GetExecutingAssemblyName()
        {
            return Assembly.GetExecutingAssembly().GetAssemblyName();
        }

        public static string GetAssemblyName(this Assembly assembly)
        {
            string name = assembly.FullName;
            return name.Substring(0, name.IndexOf(','));
        }

        public static Stream GetStream(this IResourceManager resourceManager, string relativeUri)
        {
            return resourceManager.GetStream(relativeUri, GetExecutingAssemblyName());
        }

        public static BitmapImage GetBitmap(this IResourceManager resourceManager, string relativeUri, string assemblyName)
        {
            var s = resourceManager.GetStream(relativeUri, assemblyName);
            if (s == null) return null;

            using (s)
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = s;
                bmp.EndInit();
                bmp.Freeze();
                return bmp;
            }
        }

        public static BitmapImage GetBitmap(this IResourceManager resourceManager, string relativeUri)
        {
            return resourceManager.GetBitmap(relativeUri, GetExecutingAssemblyName());
        }
    }
}