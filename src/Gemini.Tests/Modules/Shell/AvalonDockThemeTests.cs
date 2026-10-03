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
        public void ApplicationResources_VendorAndGeminiUris_LoadInDeclaredOrder(
            string themeTypeName,
            string resourceName,
            string expectedMenuBackground)
        {
            EnsurePackUriParser();
            var theme = CreateTheme(themeTypeName);

            var resourceUris = theme.ApplicationResources.ToArray();
            var vendorDictionary = LoadDictionary(resourceUris[0]);
            var geminiDictionary = LoadDictionary(resourceUris[1]);

            Assert.AreEqual(2, resourceUris.Length);
            Assert.AreEqual(
                "pack://application:,,,/AvalonDock.Themes.VS2013;component/" + resourceName,
                resourceUris[0].OriginalString);
            Assert.AreEqual(
                "pack://application:,,,/Gemini;component/Themes/VS2013/" + resourceName,
                resourceUris[1].OriginalString);
            Assert.AreEqual(2, vendorDictionary.MergedDictionaries.Count);
            Assert.AreEqual(1, geminiDictionary.MergedDictionaries.Count);
            Assert.IsInstanceOfType<Viewbox>(vendorDictionary["DockAnchorableRight"]);
            Assert.AreEqual(
                (Color)ColorConverter.ConvertFromString(expectedMenuBackground),
                Assert.IsInstanceOfType<SolidColorBrush>(
                    geminiDictionary["MenuDefaultBackground"]).Color);
            Assert.AreEqual(
                typeof(ButtonBase),
                Assert.IsInstanceOfType<Style>(
                    geminiDictionary["ToolBarButtonStyleBase"]).TargetType);
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

        // Intent: probe cross-assembly keys and types most likely to break during theme upgrades.
        [TestMethod]
        public void SelectedThemeResources_DockingMenuToolbarInspectorAndConverters_ResolveConcreteTypes()
        {
            EnsurePackUriParser();
            var themeDictionary = LoadDictionary(
                new LightTheme().ApplicationResources.First());
            var geminiDictionary = LoadDictionary(
                new LightTheme().ApplicationResources.Last());
            var inspectorResources = LoadDictionary(new Uri(
                "pack://application:,,,/Gemini.Modules.Inspector;component/Resources/Resources.xaml"));
            var inspectorTheme = LoadDictionary(new Uri(
                "pack://application:,,,/Gemini.Modules.Inspector;component/Themes/Generic.xaml"));
            var shellView = new ShellView();

            Assert.IsInstanceOfType<Viewbox>(themeDictionary["DockDocumentBottom"]);
            Assert.AreEqual(
                typeof(MenuItem),
                Assert.IsInstanceOfType<Style>(
                    geminiDictionary["MetroMenuItem"]).TargetType);
            Assert.AreEqual(
                typeof(ButtonBase),
                Assert.IsInstanceOfType<Style>(
                    geminiDictionary["ToolBarButtonStyleBase"]).TargetType);
            Assert.IsInstanceOfType<InspectorItemTemplateSelector>(
                inspectorResources["InspectorItemTemplateSelector"]);
            Assert.IsInstanceOfType<InverseBoolConverter>(
                inspectorResources["InverseBoolConverter"]);
            Assert.IsInstanceOfType<BoolToVisibilityConverter>(
                inspectorResources["BoolToVisibilityConverter"]);
            Assert.AreEqual(
                typeof(SimpleGridSplitter),
                Assert.IsInstanceOfType<Style>(
                    inspectorTheme[typeof(SimpleGridSplitter)]).TargetType);
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
            Assert.AreEqual(
                "pack://application:,,,/AvalonDock.Themes.VS2013;component/" +
                expectedResourceName,
                owner.MergedDictionaries[0].Source.OriginalString);
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
