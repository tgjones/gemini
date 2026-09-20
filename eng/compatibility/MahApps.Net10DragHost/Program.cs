using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using System.Reflection;
using MahApps.Metro.Controls;

namespace MahApps.Net10DragHost;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        var application = new Application
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown
        };

        AddMahAppsResources(application);

        var smokeTest = args.Contains("--smoke", StringComparer.OrdinalIgnoreCase);
        var window = CreateWindow(smokeTest);

        if (smokeTest)
        {
            window.Loaded += (_, _) =>
            {
                var handle = new WindowInteropHelper(window).Handle;
                application.Dispatcher.BeginInvoke(
                    () => application.Shutdown(handle == IntPtr.Zero ? 1 : 0),
                    DispatcherPriority.ApplicationIdle);
            };
        }

        return application.Run(window);
    }

    private static void AddMahAppsResources(Application application)
    {
        var resourceUris = new[]
        {
            "pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml",
            "pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml",
            "pack://application:,,,/MahApps.Metro;component/Styles/Themes/Light.Blue.xaml"
        };

        foreach (var resourceUri in resourceUris)
        {
            application.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(resourceUri, UriKind.Absolute)
            });
        }
    }

    private static MetroWindow CreateWindow(bool smokeTest)
    {
        var packageVersion = typeof(Program).Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .Single(attribute => attribute.Key == "MahAppsPackageVersion")
            .Value;
        var assemblyVersion = typeof(MetroWindow).Assembly.GetName().Version;
        var runtimeVersion = Environment.Version;
        var status = new TextBlock
        {
            Margin = new Thickness(0, 16, 0, 0),
            TextWrapping = TextWrapping.Wrap
        };

        var window = new MetroWindow
        {
            Title = $"MahApps {packageVersion} .NET 10 drag host",
            Width = 720,
            Height = 420,
            MinWidth = 520,
            MinHeight = 320,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.CanResizeWithGrip,
            GlowBrush = Brushes.DodgerBlue,
            NonActiveGlowBrush = Brushes.Gray,
            Content = new StackPanel
            {
                Margin = new Thickness(32),
                Children =
                {
                    new TextBlock
                    {
                        FontSize = 22,
                        FontWeight = FontWeights.SemiBold,
                        Text = "MetroWindow title-drag compatibility"
                    },
                    new TextBlock
                    {
                        Margin = new Thickness(0, 12, 0, 0),
                        Text = $"Runtime: .NET {runtimeVersion}{Environment.NewLine}" +
                               $"MahApps.Metro package: {packageVersion}{Environment.NewLine}" +
                               $"MahApps.Metro assembly: {assemblyVersion}"
                    },
                    new TextBlock
                    {
                        Margin = new Thickness(0, 16, 0, 0),
                        TextWrapping = TextWrapping.Wrap,
                        Text = "1. Drag this window by its title bar and confirm that it moves." +
                               $"{Environment.NewLine}2. Maximize it, then drag the title bar and confirm that it restores and moves." +
                               $"{Environment.NewLine}3. Minimize/reactivate, resize, and verify the caption buttons."
                    },
                    status
                }
            }
        };

        var locationChanges = 0;
        window.LocationChanged += (_, _) =>
        {
            locationChanges++;
            UpdateStatus(window, status, locationChanges, smokeTest);
        };
        window.StateChanged += (_, _) => UpdateStatus(window, status, locationChanges, smokeTest);
        window.Loaded += (_, _) => UpdateStatus(window, status, locationChanges, smokeTest);
        window.Closed += (_, _) => Application.Current.Shutdown();

        return window;
    }

    private static void UpdateStatus(
        MetroWindow window,
        TextBlock status,
        int locationChanges,
        bool smokeTest)
    {
        status.Text = smokeTest
            ? "Smoke mode: verifying that the MetroWindow creates a native handle."
            : $"Window state: {window.WindowState}{Environment.NewLine}" +
              $"Location changes observed: {locationChanges}{Environment.NewLine}" +
              $"Current position: ({window.Left:0}, {window.Top:0})";
    }
}
