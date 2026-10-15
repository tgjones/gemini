using System;
using System.Collections.Generic;
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
using Gemini.Modules.CodeEditor;
using Gemini.Modules.CodeEditor.Controls;
using Gemini.Modules.CodeEditor.Views;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid;

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
            var expectedAvalonEditVersion = new Version(6, 3, 1, 120);
            var expectedToolkitVersion = new Version(5, 0, 129, 0);
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

            var codeEditor = new ExternalCodeEditor();
            ICodeEditorView codeEditorView = new ExternalCodeEditorView(codeEditor);
            ILanguageDefinition languageDefinition = new ExternalLanguageDefinition(
                HighlightingManager.Instance.GetDefinition("C#"));
            var avalonEditAssemblyName = typeof(TextEditor).Assembly.GetName();
            Console.WriteLine(
                "{0} {1}",
                avalonEditAssemblyName.Name,
                avalonEditAssemblyName.Version);
            var toolkitAssemblyName = typeof(PropertyGrid).Assembly.GetName();
            var toolkitPublicKeyToken = BitConverter.ToString(
                    toolkitAssemblyName.GetPublicKeyToken())
                .Replace("-", string.Empty)
                .ToLowerInvariant();
            Console.WriteLine(
                "{0} {1}",
                toolkitAssemblyName.Name,
                toolkitAssemblyName.Version);
            var colorPicker = new ColorPicker
            {
                SelectedColor = System.Windows.Media.Colors.CornflowerBlue
            };
            var colorCanvas = new ColorCanvas
            {
                SelectedColor = System.Windows.Media.Colors.Goldenrod
            };
            var collectionControl = new CollectionControl
            {
                ItemsSource = new List<string> { "one" },
                ItemsSourceType = typeof(List<string>)
            };
            var geminiTheme = new ResourceDictionary
            {
                Source = new Uri(
                    "pack://application:,,,/Gemini;component/Themes/VS2013/LightTheme.xaml")
            };

            return typeof(WindowBase).Assembly.GetName().Name == "Gemini"
                && typeof(Screen).Assembly.GetName().Name == "Caliburn.Micro.Core"
                && typeof(BootstrapperBase).Assembly.GetName().Name == "Caliburn.Micro.Platform"
                && avalonEditAssemblyName.Name == "ICSharpCode.AvalonEdit"
                && avalonEditAssemblyName.Version == expectedAvalonEditVersion
                && codeEditorView.TextEditor == codeEditor
                && languageDefinition.SyntaxHighlighting != null
                && toolkitAssemblyName.Name == "DotNetProjects.Wpf.Extended.Toolkit"
                && toolkitAssemblyName.Version == expectedToolkitVersion
                && toolkitPublicKeyToken == "3e4669d2f30244f4"
                && typeof(ColorPicker).Assembly == typeof(PropertyGrid).Assembly
                && typeof(ColorCanvas).Assembly == typeof(PropertyGrid).Assembly
                && typeof(CollectionControl).Assembly == typeof(PropertyGrid).Assembly
                && colorPicker.SelectedColor == System.Windows.Media.Colors.CornflowerBlue
                && colorCanvas.SelectedColor == System.Windows.Media.Colors.Goldenrod
                && collectionControl.Items.Count == 1
                && geminiTheme[typeof(ColorCanvas)] is Style
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

        private sealed class ExternalCodeEditor : CodeEditor
        {
        }

        private sealed class ExternalCodeEditorView : ICodeEditorView
        {
            internal ExternalCodeEditorView(TextEditor textEditor)
            {
                TextEditor = textEditor;
            }

            public TextEditor TextEditor { get; }

            public void ApplySettings()
            {
                ((CodeEditor)TextEditor).ApplySettings();
            }
        }

        private sealed class ExternalLanguageDefinition : ILanguageDefinition
        {
            internal ExternalLanguageDefinition(
                IHighlightingDefinition syntaxHighlighting)
            {
                Name = "External C#";
                FileExtensions = new[] { ".external-cs" };
                SyntaxHighlighting = syntaxHighlighting;
            }

            public string Name { get; }

            public IEnumerable<string> FileExtensions { get; set; }

            public IHighlightingDefinition SyntaxHighlighting { get; }

            public string CustomSyntaxHighlightingFileName { get; set; }
        }
    }
}
