using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using AvalonDock.Converters;
using Gemini.Framework.Themes;
using Gemini.Modules.Inspector.Controls;
using Gemini.Modules.Shell.Converters;
using Gemini.Modules.Shell.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xceed.Wpf.Toolkit.Core.Converters;

namespace Gemini.Tests.Modules.Shell
{
    [STATestClass]
    [DoNotParallelize]
    public sealed class AvalonDockThemeTests
    {
        // Intent: preserve vendor-first resource ordering so Gemini overrides remain authoritative.
        [TestMethod]
        [DataRow("LightTheme", "LightTheme.xaml", "#FFEEEEF2")]
        [DataRow("DarkTheme", "DarkTheme.xaml", "#FF2D2D30")]
        [DataRow("BlueTheme", "BlueTheme.xaml", "#FFD6DBE9")]
        public void ApplicationResources_GeneratedVendorAndGeminiUri_LoadInDeclaredOrder(
            string themeTypeName,
            string resourceName,
            string expectedMenuBackground)
        {
            EnsurePackUriParser();
            var theme = CreateTheme(themeTypeName);
            using (var host = new ThemeHost())
            {
                var manager = new ThemeManager(new[] { theme });
                try
                {
                    Assert.IsTrue(manager.SetCurrentTheme(themeTypeName));

                    var resourceUris = theme.ApplicationResources.ToArray();
                    var owner = Application.Current.Resources.MergedDictionaries.Last();
                    var vendorDictionary = owner.MergedDictionaries[0];
                    var geminiDictionary = owner.MergedDictionaries[1];

                    Assert.AreEqual(1, resourceUris.Length);
                    Assert.AreEqual(
                        "pack://application:,,,/Gemini;component/Themes/VS2013/" +
                        resourceName,
                        resourceUris[0].OriginalString);
                    Assert.IsNull(vendorDictionary.Source);
                    Assert.AreEqual(resourceUris[0], geminiDictionary.Source);
                    Assert.IsInstanceOfType<Viewbox>(
                        vendorDictionary["DockAnchorableRight"]);
                    Assert.AreEqual(
                        (Color)ColorConverter.ConvertFromString(expectedMenuBackground),
                        Assert.IsInstanceOfType<SolidColorBrush>(
                            Application.Current.Resources[
                                "MenuDefaultBackground"]).Color);
                    Assert.AreEqual(
                        typeof(ButtonBase),
                        Assert.IsInstanceOfType<Style>(
                            Application.Current.Resources[
                                "ToolBarButtonStyleBase"]).TargetType);
                }
                finally
                {
                    DetachThemeSettingsListener(manager);
                }
            }
        }

        // Intent: keep failed lookups inert and repeated live switches deterministic.
        [TestMethod]
        public void SetCurrentTheme_MissingWindowUnknownThemeAndLiveSequence_PreserveExpectedState()
        {
            EnsurePackUriParser();
            using (var host = new ThemeHost())
            {
                var themes = new ITheme[]
                {
                    new LightTheme(),
                    new DarkTheme(),
                    new BlueTheme()
                };
                var manager = new ThemeManager(themes);
                try
                {
                    var changedThemes = new List<Type>();
                    manager.CurrentThemeChanged += (sender, args) =>
                        changedThemes.Add(manager.CurrentTheme.GetType());

                    Application.Current.MainWindow = null;
                    Assert.IsFalse(manager.SetCurrentTheme(nameof(LightTheme)));
                    Assert.IsNull(manager.CurrentTheme);
                    Assert.AreEqual(0, changedThemes.Count);
                    Application.Current.MainWindow = host.Window;

                    Assert.IsFalse(manager.SetCurrentTheme("UnknownTheme"));
                    Assert.IsNull(manager.CurrentTheme);
                    Assert.AreEqual(0, changedThemes.Count);

                    AssertThemeSwitch(
                        manager,
                        nameof(LightTheme),
                        typeof(LightTheme),
                        "LightTheme.xaml",
                        "#FFEEEEF2",
                        "#FFEEEEF2");
                    AssertThemeSwitch(
                        manager,
                        nameof(DarkTheme),
                        typeof(DarkTheme),
                        "DarkTheme.xaml",
                        "#FF2D2D30",
                        "#FF2D2D30");
                    AssertThemeSwitch(
                        manager,
                        nameof(BlueTheme),
                        typeof(BlueTheme),
                        "BlueTheme.xaml",
                        "#FFD6DBE9",
                        "#FFCFD6E5");
                    AssertThemeSwitch(
                        manager,
                        nameof(LightTheme),
                        typeof(LightTheme),
                        "LightTheme.xaml",
                        "#FFEEEEF2",
                        "#FFEEEEF2");

                    var applicationThemeDictionary =
                        Application.Current.Resources.MergedDictionaries.Single(
                            dictionary => dictionary.MergedDictionaries.Count == 2);
                    Assert.IsTrue(manager.SetCurrentTheme(nameof(LightTheme)));
                    Assert.AreSame(
                        applicationThemeDictionary,
                        Application.Current.Resources.MergedDictionaries.Single(
                            dictionary => dictionary.MergedDictionaries.Count == 2));
                    CollectionAssert.AreEqual(
                        new[]
                        {
                            typeof(LightTheme),
                            typeof(DarkTheme),
                            typeof(BlueTheme),
                            typeof(LightTheme)
                        },
                        changedThemes);
                    Assert.AreEqual(host.InitialApplicationDictionaryCount + 1,
                        Application.Current.Resources.MergedDictionaries.Count);
                    Assert.AreEqual(0,
                        host.WindowThemeDictionary.MergedDictionaries.Count);
                }
                finally
                {
                    DetachThemeSettingsListener(manager);
                }
            }
        }

        // Intent: preserve the public URI-only ITheme extension contract for third-party themes.
        [TestMethod]
        public void SetCurrentTheme_ExternalUriOnlyTheme_DoesNotInjectVendorResources()
        {
            EnsurePackUriParser();
            using (var host = new ThemeHost())
            {
                var manager = new ThemeManager(new ITheme[]
                {
                    new ExternalUriOnlyTheme()
                });
                try
                {
                    Assert.IsTrue(
                        manager.SetCurrentTheme(nameof(ExternalUriOnlyTheme)));

                    var owner =
                        Application.Current.Resources.MergedDictionaries.Last();
                    Assert.AreEqual(1, owner.MergedDictionaries.Count);
                    Assert.AreEqual(
                        ExternalUriOnlyTheme.ResourceUri,
                        owner.MergedDictionaries[0].Source);
                    Assert.IsFalse(owner.Contains("DockAnchorableRight"));
                    Assert.AreEqual(
                        (Color)ColorConverter.ConvertFromString("#FFEEEEF2"),
                        Assert.IsInstanceOfType<SolidColorBrush>(
                            owner["MenuDefaultBackground"]).Color);
                }
                finally
                {
                    DetachThemeSettingsListener(manager);
                }
            }
        }

        // Intent: prevent a malformed theme from exposing partial resources or a false change event.
        [TestMethod]
        public void SetCurrentTheme_FailedMaterialization_PreservesCurrentThemeAndResources()
        {
            EnsurePackUriParser();
            using (var host = new ThemeHost())
            {
                var lightTheme = new LightTheme();
                var manager = new ThemeManager(new ITheme[]
                {
                    lightTheme,
                    new MissingWindowResourceTheme()
                });
                try
                {
                    int changeCount = 0;
                    manager.CurrentThemeChanged += (sender, args) => changeCount++;
                    Assert.IsTrue(manager.SetCurrentTheme(nameof(LightTheme)));
                    var owner =
                        Application.Current.Resources.MergedDictionaries.Last();
                    var applicationResources =
                        owner.MergedDictionaries.ToArray();
                    var windowResources =
                        host.WindowThemeDictionary.MergedDictionaries.ToArray();
                    Exception failure = null;

                    try
                    {
                        manager.SetCurrentTheme(
                            nameof(MissingWindowResourceTheme));
                    }
                    catch (Exception exception)
                    {
                        failure = exception;
                    }

                    Assert.IsNotNull(failure);
                    Assert.AreSame(lightTheme, manager.CurrentTheme);
                    Assert.AreEqual(1, changeCount);
                    CollectionAssert.AreEqual(
                        applicationResources,
                        owner.MergedDictionaries.ToArray());
                    CollectionAssert.AreEqual(
                        windowResources,
                        host.WindowThemeDictionary.MergedDictionaries.ToArray());
                }
                finally
                {
                    DetachThemeSettingsListener(manager);
                }
            }
        }

        // Intent: probe cross-assembly keys and types most likely to break during theme upgrades.
        [TestMethod]
        public void SelectedThemeResources_DockingMenuToolbarInspectorAndConverters_ResolveConcreteTypes()
        {
            EnsurePackUriParser();
            using (var host = new ThemeHost())
            {
                var manager = new ThemeManager(new ITheme[]
                {
                    new LightTheme()
                });
                try
                {
                    Assert.IsTrue(manager.SetCurrentTheme(nameof(LightTheme)));
                    var inspectorResources = LoadDictionary(new Uri(
                        "pack://application:,,,/Gemini.Modules.Inspector;component/Resources/Resources.xaml"));
                    var inspectorTheme = LoadDictionary(new Uri(
                        "pack://application:,,,/Gemini.Modules.Inspector;component/Themes/Generic.xaml"));
                    var shellView = new ShellView();

                    Assert.IsInstanceOfType<Viewbox>(
                        Application.Current.Resources["DockDocumentBottom"]);
                    Assert.AreEqual(
                        typeof(MenuItem),
                        Assert.IsInstanceOfType<Style>(
                            Application.Current.Resources[
                                "MetroMenuItem"]).TargetType);
                    Assert.AreEqual(
                        typeof(ButtonBase),
                        Assert.IsInstanceOfType<Style>(
                            Application.Current.Resources[
                                "ToolBarButtonStyleBase"]).TargetType);
                    Assert.IsInstanceOfType<InspectorItemTemplateSelector>(
                        inspectorResources["InspectorItemTemplateSelector"]);
                    Assert.IsInstanceOfType<InverseBoolConverter>(
                        inspectorResources["InverseBoolConverter"]);
                    Assert.IsInstanceOfType<BoolToVisibilityConverter>(
                        inspectorResources["BoolToVisibilityConverter"]);
                    Assert.AreEqual(
                        typeof(SimpleGridSplitter),
                        Assert.IsInstanceOfType<Style>(
                            inspectorTheme[
                                typeof(SimpleGridSplitter)]).TargetType);
                    Assert.AreEqual(
                        typeof(NumericTextBox),
                        Assert.IsInstanceOfType<Style>(
                            inspectorTheme[typeof(NumericTextBox)]).TargetType);
                    Assert.IsInstanceOfType<NullableValueConverter>(
                        shellView.Resources["NullableValueConverter"]);
                    Assert.IsInstanceOfType<TruncateMiddleConverter>(
                        shellView.Resources["TruncateMiddleConverter"]);
                    Assert.IsInstanceOfType<BoolToVisibilityConverter>(
                        shellView.Resources["BoolToVisibilityConverter"]);
                }
                finally
                {
                    DetachThemeSettingsListener(manager);
                }
            }
        }

        private static void AssertThemeSwitch(
            ThemeManager manager,
            string themeName,
            Type expectedThemeType,
            string expectedResourceName,
            string expectedMenuBackground,
            string expectedToolbarBackground)
        {
            var changed = manager.SetCurrentTheme(themeName);

            Assert.IsTrue(changed);
            Assert.AreEqual(expectedThemeType, manager.CurrentTheme.GetType());
            var owner = Application.Current.Resources.MergedDictionaries
                .Single(dictionary => dictionary.MergedDictionaries.Count == 2);
            Assert.IsNull(owner.MergedDictionaries[0].Source);
            Assert.IsInstanceOfType<Viewbox>(
                owner.MergedDictionaries[0]["DockAnchorableRight"]);
            Assert.AreEqual(
                "pack://application:,,,/Gemini;component/Themes/VS2013/" +
                expectedResourceName,
                owner.MergedDictionaries[1].Source.OriginalString);
            Assert.AreEqual(
                (Color)ColorConverter.ConvertFromString(expectedMenuBackground),
                Assert.IsInstanceOfType<SolidColorBrush>(
                    Application.Current.Resources["MenuDefaultBackground"]).Color);
            Assert.AreEqual(
                (Color)ColorConverter.ConvertFromString(expectedToolbarBackground),
                Assert.IsInstanceOfType<SolidColorBrush>(
                    Application.Current.Resources["ToolbarDefaultBackground"]).Color);
            Assert.IsInstanceOfType<Viewbox>(
                Application.Current.Resources["DockAnchorableRight"]);
        }

        private static ITheme CreateTheme(string themeTypeName)
        {
            switch (themeTypeName)
            {
                case nameof(LightTheme):
                    return new LightTheme();
                case nameof(DarkTheme):
                    return new DarkTheme();
                case nameof(BlueTheme):
                    return new BlueTheme();
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(themeTypeName),
                        themeTypeName,
                        "Unknown built-in theme.");
            }
        }

        private sealed class ExternalUriOnlyTheme : ITheme
        {
            public static readonly Uri ResourceUri = new Uri(
                "pack://application:,,,/Gemini;component/Themes/VS2013/LightTheme.xaml");

            public string Name => "External URI-only";

            public IEnumerable<Uri> ApplicationResources
            {
                get { yield return ResourceUri; }
            }

            public IEnumerable<Uri> MainWindowResources
            {
                get { yield break; }
            }
        }

        private sealed class MissingWindowResourceTheme : ITheme
        {
            public string Name => "Missing window resource";

            public IEnumerable<Uri> ApplicationResources
            {
                get { yield return ExternalUriOnlyTheme.ResourceUri; }
            }

            public IEnumerable<Uri> MainWindowResources
            {
                get
                {
                    yield return new Uri(
                        "pack://application:,,,/Gemini;component/Themes/VS2013/Missing.xaml");
                }
            }
        }

        private static ResourceDictionary LoadDictionary(Uri uri)
        {
            return new ResourceDictionary { Source = uri };
        }

        private static void EnsurePackUriParser()
        {
            _ = Application.ResourceAssembly;
        }

        private static void DetachThemeSettingsListener(ThemeManager manager)
        {
            var flags = System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance;
            var managerField = typeof(ThemeManager).GetField(
                "_settingsEventManager",
                flags);
            if (managerField == null)
            {
                throw new InvalidOperationException(
                    "ThemeManager settings event field was not found.");
            }

            var eventManager = managerField.GetValue(manager);
            var eventManagerType = eventManager.GetType();
            var settingsField = eventManagerType.GetField(
                "_applicationSettings",
                flags);
            var listenersField = eventManagerType.GetField(
                "_eventListeners",
                flags);
            if (settingsField == null || listenersField == null)
            {
                throw new InvalidOperationException(
                    "Theme settings listener fields were not found.");
            }

            var settings = (INotifyPropertyChanged)settingsField.GetValue(eventManager);
            var listeners = (IList)listenersField.GetValue(eventManager);
            foreach (IWeakEventListener listener in listeners)
            {
                PropertyChangedEventManager.RemoveListener(
                    settings,
                    listener,
                    "ThemeName");
            }
            listeners.Clear();
        }

        private sealed class ThemeHost : IDisposable
        {
            private readonly Application _application;
            private readonly bool _ownsApplication;
            private readonly Window _previousMainWindow;
            private readonly ResourceDictionary[] _initialApplicationDictionaries;

            public ThemeHost()
            {
                _application = Application.Current;
                if (_application == null)
                {
                    _application = new Application
                    {
                        ShutdownMode = ShutdownMode.OnExplicitShutdown
                    };
                    _ownsApplication = true;
                }

                _previousMainWindow = _application.MainWindow;
                _initialApplicationDictionaries =
                    _application.Resources.MergedDictionaries.ToArray();
                InitialApplicationDictionaryCount =
                    _initialApplicationDictionaries.Length;
                Window = new Window();
                WindowThemeDictionary = new ResourceDictionary();
                Window.Resources.MergedDictionaries.Add(WindowThemeDictionary);
                _application.MainWindow = Window;
            }

            public int InitialApplicationDictionaryCount { get; }

            public Window Window { get; }

            public ResourceDictionary WindowThemeDictionary { get; }

            public void Dispose()
            {
                _application.MainWindow = _previousMainWindow;
                _application.Resources.MergedDictionaries.Clear();
                foreach (var dictionary in _initialApplicationDictionaries)
                    _application.Resources.MergedDictionaries.Add(dictionary);

                if (_ownsApplication)
                    ResetApplicationSingleton(_application);
            }

            private static void ResetApplicationSingleton(Application application)
            {
                var flags = System.Reflection.BindingFlags.NonPublic |
                            System.Reflection.BindingFlags.Static;
                var applicationType = typeof(Application);
                var instanceField = applicationType.GetField("_appInstance", flags);
                var createdField = applicationType.GetField(
                    "_appCreatedInThisAppDomain",
                    flags);
                var shuttingDownField = applicationType.GetField(
                    "_isShuttingDown",
                    flags);
                if (instanceField == null ||
                    createdField == null ||
                    shuttingDownField == null)
                {
                    throw new InvalidOperationException(
                        "The WPF Application singleton fields required for test isolation were not found.");
                }

                Assert.AreSame(application, instanceField.GetValue(null));
                instanceField.SetValue(null, null);
                createdField.SetValue(null, false);
                shuttingDownField.SetValue(null, false);
            }
        }
    }
}
