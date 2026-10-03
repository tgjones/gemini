using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using AvalonDock;
using AvalonDock.Layout.Serialization;
using Caliburn.Micro;
using Gemini.Framework;

namespace Caliburn5PackageHost
{
    internal static class Program
    {
        [STAThread]
        private static int Main()
        {
            var extension = new LifecycleExtension();
            extension.Activated += extension.OnActivatedAsync;

            var manager = new DockingManager();
            var serializer = new XmlLayoutSerializer(manager);
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream);
                if (stream.Length == 0)
                    return 1;
            }

            var theme = new ResourceDictionary
            {
                Source = new Uri(
                    "pack://application:,,,/AvalonDock.Themes.VS2013;component/LightTheme.xaml",
                    UriKind.Absolute)
            };
            var themeAssembly = Assembly.Load("AvalonDock.Themes.VS2013");
            var expectedAvalonDockVersion = new Version(4, 74, 1, 0);

            return typeof(WindowBase).Assembly.GetName().Name == "Gemini"
                && typeof(Screen).Assembly.GetName().Name == "Caliburn.Micro.Core"
                && typeof(BootstrapperBase).Assembly.GetName().Name == "Caliburn.Micro.Platform"
                && typeof(DockingManager).Assembly.GetName().Version == expectedAvalonDockVersion
                && themeAssembly.GetName().Version == expectedAvalonDockVersion
                && manager.Layout != null
                && theme.MergedDictionaries.Count > 0
                ? 0
                : 1;
        }

        private sealed class LifecycleExtension : WindowBase
        {
            public Task OnActivatedAsync(object sender, ActivationEventArgs eventArgs)
                => Task.CompletedTask;
        }
    }
}
