using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AvalonDock;
using AvalonDock.Serializer.Xml;
using AvalonDock.Themes;
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

            var theme = new Vs2013LightTheme().ThemeResourceDictionary;
            var expectedAvalonDockVersion = new Version(5, 0, 1, 0);
            var avalonDockAssemblies = new[]
            {
                typeof(DockingManager).Assembly,
                Assembly.Load("AvalonDock.Core"),
                typeof(XmlLayoutSerializer).Assembly,
                Assembly.Load("AvalonDock.Themes.VS"),
                typeof(Vs2013LightTheme).Assembly
            };
            var expectedAvalonDockAssemblyNames = new[]
            {
                "AvalonDock",
                "AvalonDock.Core",
                "AvalonDock.Serializer.Xml",
                "AvalonDock.Themes.VS",
                "AvalonDock.Themes.VS2013"
            };
            for (int i = 0; i < avalonDockAssemblies.Length; i++)
            {
                var assemblyName = avalonDockAssemblies[i].GetName();
                Console.WriteLine(
                    "{0} {1}",
                    assemblyName.Name,
                    assemblyName.Version);
                if (assemblyName.Name != expectedAvalonDockAssemblyNames[i] ||
                    assemblyName.Version != expectedAvalonDockVersion)
                {
                    return 1;
                }
            }

            return typeof(WindowBase).Assembly.GetName().Name == "Gemini"
                && typeof(Screen).Assembly.GetName().Name == "Caliburn.Micro.Core"
                && typeof(BootstrapperBase).Assembly.GetName().Name == "Caliburn.Micro.Platform"
                && manager.Layout != null
                && theme["DockAnchorableRight"] is Viewbox
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
