using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Gemini.Modules.Inspector.Inspectors;
using Gemini.Modules.PropertyGrid.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Gemini.Tests.Modules.Toolkit
{
    [STATestClass]
    [DoNotParallelize]
    public sealed class ExtendedWpfToolkitCompatibilityTests
    {
        private static readonly Version ExpectedToolkitVersion =
            new Version(4, 6, 78, 0);

        [TestMethod]
        public void AssemblyIdentityAndRequiredTemplateParts_RemainStable()
        {
            var assemblyName = typeof(PropertyGrid).Assembly.GetName();

            Assert.AreEqual(
                "DotNetProjects.Wpf.Extended.Toolkit",
                assemblyName.Name);
            Assert.AreEqual(ExpectedToolkitVersion, assemblyName.Version);
            Assert.AreEqual(
                "3e4669d2f30244f4",
                GetPublicKeyToken(assemblyName));
            Assert.AreSame(typeof(PropertyGrid).Assembly, typeof(ColorPicker).Assembly);
            Assert.AreSame(typeof(PropertyGrid).Assembly, typeof(ColorCanvas).Assembly);
            Assert.AreSame(
                typeof(PropertyGrid).Assembly,
                typeof(CollectionControl).Assembly);

            AssertTemplateParts(
                typeof(PropertyGrid),
                "PART_DragThumb",
                "PART_PropertyItemsControl");
            AssertTemplateParts(
                typeof(ColorPicker),
                "PART_AvailableColors",
                "PART_ColorPickerPalettePopup",
                "PART_ColorPickerToggleButton",
                "PART_RecentColors",
                "PART_StandardColors");
            AssertTemplateParts(
                typeof(ColorCanvas),
                "PART_ColorShadingCanvas",
                "PART_ColorShadeSelector",
                "PART_HexadecimalTextBox",
                "PART_SpectrumSlider");
            AssertTemplateParts(
                typeof(CollectionControl),
                "PART_ListBox",
                "PART_NewItemTypesComboBox",
                "PART_PropertyGrid");
        }

        [TestMethod]
        [Timeout(10000)]
        public void PropertyGridModule_SelectedObjectsAndRepresentativeEditorsRemainUsable()
        {
            var first = new PropertyGridSample
            {
                Text = "first",
                ReadOnlyText = "read only",
                Nested = new NestedSample { Name = "nested" },
                OptionalMode = SampleMode.Second,
                Brush = Brushes.CornflowerBlue,
                Address = IPAddress.Loopback,
                Array = new[] { "one", "two" },
                Items = new List<string> { "alpha", "beta" }
            };
            var viewModel = new PropertyGridBindingModel { SelectedObject = first };
            var view = new PropertyGridView { DataContext = viewModel };

            using (var host = new ControlHost(view))
            {
                DrainDispatcher();
                var propertyGrid = (PropertyGrid)view.FindName("_propertyGrid");
                propertyGrid.IsVirtualizing = false;
                host.Window.UpdateLayout();
                DrainDispatcher();

                Assert.IsFalse(propertyGrid.IsCategorized);
                Assert.AreSame(first, propertyGrid.SelectedObject);
                var firstItems = GetPropertyItems(propertyGrid);
                AssertRepresentativeProperties(first, firstItems);

                firstItems[nameof(PropertyGridSample.Text)].Value = "changed";
                DrainDispatcher();
                Assert.AreEqual("changed", first.Text);

                var second = new PropertyGridSample
                {
                    Text = "second",
                    ReadOnlyText = "replacement",
                    Nested = new NestedSample { Name = "replacement nested" },
                    OptionalMode = null,
                    Brush = Brushes.Goldenrod,
                    Address = IPAddress.IPv6Loopback,
                    Array = new[] { "replacement" },
                    Items = new List<string> { "replacement" }
                };
                propertyGrid.SelectedObject = second;
                DrainDispatcher();

                var secondItems = GetPropertyItems(propertyGrid);
                AssertRepresentativeProperties(second, secondItems);
                Assert.AreEqual(
                    "second",
                    secondItems[nameof(PropertyGridSample.Text)].Value);

                propertyGrid.SelectedObject = null;
                DrainDispatcher();
                Assert.AreEqual(0, propertyGrid.Properties.Count);
            }
        }

        [TestMethod]
        [Timeout(10000)]
        public void CollectionControl_PersistsMutableListChanges()
        {
            var first = new CollectionItemSample { Name = "one" };
            var second = new CollectionItemSample { Name = "two" };
            var source = new List<CollectionItemSample> { first, second };
            var control = new CollectionControl
            {
                ItemsSource = source,
                ItemsSourceType = source.GetType(),
                NewItemTypes = new List<Type> { typeof(CollectionItemSample) }
            };

            using (var host = new ControlHost(control))
            {
                DrainDispatcher();
                CollectionAssert.AreEqual(
                    source,
                    control.Items.Cast<CollectionItemSample>().ToList());

                ApplicationCommands.New.Execute(
                    typeof(CollectionItemSample),
                    control);
                DrainDispatcher();
                var added = (CollectionItemSample)control.Items.Last();
                added.Name = "three";
                ComponentCommands.MoveUp.Execute(added, control);
                ComponentCommands.MoveUp.Execute(added, control);
                ApplicationCommands.Delete.Execute(second, control);

                Assert.IsTrue(control.PersistChanges());
                CollectionAssert.AreEqual(
                    new[] { added, first },
                    source);
            }
        }

        [TestMethod]
        [Timeout(10000)]
        public void ColorPickerCanvasAndInspectorView_KeepPopupAndTemplateContracts()
        {
            EnsurePackUriParser();
            var picker = new ColorPicker
            {
                SelectedColor = Color.FromArgb(0x80, 0x11, 0x22, 0x33),
                ShowTabHeaders = true,
                StandardTabHeader = "Standard",
                AdvancedTabHeader = "Advanced",
                UsingAlphaChannel = true
            };
            var canvas = new ColorCanvas
            {
                SelectedColor = Color.FromArgb(0x40, 0x44, 0x55, 0x66),
                UsingAlphaChannel = true
            };
            canvas.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(
                    "pack://application:,,,/Gemini;component/Themes/VS2013/LightTheme.xaml")
            });
            var panel = new StackPanel();
            panel.Children.Add(picker);
            panel.Children.Add(canvas);

            var openedCount = 0;
            var closedCount = 0;
            picker.Opened += delegate { openedCount++; };
            picker.Closed += delegate { closedCount++; };

            using (var host = new ControlHost(panel))
            {
                DrainDispatcher();
                picker.ApplyTemplate();
                canvas.ApplyTemplate();
                host.Window.UpdateLayout();
                DrainDispatcher();

                AssertTemplateInstances(picker);
                AssertTemplateInstances(canvas);
                Assert.AreEqual((byte)0x40, canvas.A);
                Assert.AreEqual((byte)0x44, canvas.R);
                Assert.AreEqual((byte)0x55, canvas.G);
                Assert.AreEqual((byte)0x66, canvas.B);

                picker.IsOpen = true;
                DrainDispatcher();
                Assert.AreEqual(1, openedCount);
                Assert.IsTrue(picker.IsOpen);

                var popup = (Popup)picker.Template.FindName(
                    "PART_ColorPickerPalettePopup",
                    picker);
                Assert.IsNotNull(popup);
                Assert.IsTrue(popup.IsOpen);
                var tabs = FindVisualDescendants<TabControl>(popup.Child)
                    .Single();
                Assert.IsTrue(tabs.Items.Count >= 2);
                tabs.SelectedIndex = 1;
                DrainDispatcher();
                Assert.IsTrue(picker.IsOpen);

                picker.IsOpen = false;
                DrainDispatcher();
                Assert.AreEqual(1, closedCount);
                Assert.IsFalse(popup.IsOpen);
            }

            var inspectorView = new ColorEditorView();
            using (var host = new ControlHost(inspectorView))
            {
                DrainDispatcher();
                var inspectorPicker =
                    FindVisualDescendants<ColorPicker>(inspectorView).Single();

                Assert.IsFalse(string.IsNullOrWhiteSpace(
                    inspectorPicker.StandardTabHeader));
                Assert.IsFalse(string.IsNullOrWhiteSpace(
                    inspectorPicker.AdvancedTabHeader));
                Assert.IsFalse(string.IsNullOrWhiteSpace(
                    inspectorPicker.AvailableColorsHeader));
                Assert.IsFalse(string.IsNullOrWhiteSpace(
                    inspectorPicker.RecentColorsHeader));
            }
        }

        private static void AssertRepresentativeProperties(
            PropertyGridSample sample,
            IReadOnlyDictionary<string, PropertyItem> items)
        {
            var expectedNames = new[]
            {
                nameof(PropertyGridSample.Text),
                nameof(PropertyGridSample.ReadOnlyText),
                nameof(PropertyGridSample.Nested),
                nameof(PropertyGridSample.OptionalMode),
                nameof(PropertyGridSample.Brush),
                nameof(PropertyGridSample.Address),
                nameof(PropertyGridSample.Array),
                nameof(PropertyGridSample.Items)
            };
            foreach (var name in expectedNames)
                Assert.IsTrue(items.ContainsKey(name), "Missing property " + name + ".");

            Assert.IsTrue(items[nameof(PropertyGridSample.ReadOnlyText)].IsReadOnly);
            Assert.IsTrue(items[nameof(PropertyGridSample.Nested)].IsExpandable);
            Assert.AreEqual(
                sample.OptionalMode,
                items[nameof(PropertyGridSample.OptionalMode)].Value);
            Assert.AreEqual(
                sample.Brush,
                items[nameof(PropertyGridSample.Brush)].Value);
            Assert.AreEqual(
                sample.Address,
                items[nameof(PropertyGridSample.Address)].Value);
            Assert.IsNotNull(items[nameof(PropertyGridSample.Text)].Editor);
            Assert.IsNotNull(items[nameof(PropertyGridSample.OptionalMode)].Editor);
            Assert.IsNotNull(items[nameof(PropertyGridSample.Brush)].Editor);
            Assert.IsNotNull(items[nameof(PropertyGridSample.Address)].Editor);
            Assert.IsNotNull(items[nameof(PropertyGridSample.Array)].Editor);
            Assert.IsNotNull(items[nameof(PropertyGridSample.Items)].Editor);
        }

        private static IReadOnlyDictionary<string, PropertyItem> GetPropertyItems(
            PropertyGrid propertyGrid)
        {
            return propertyGrid.Properties
                .Cast<PropertyItem>()
                .Where(item => item.PropertyDescriptor != null)
                .ToDictionary(item => item.PropertyDescriptor.Name);
        }

        private static void AssertTemplateInstances(ColorPicker picker)
        {
            foreach (var name in new[]
            {
                "PART_AvailableColors",
                "PART_ColorPickerPalettePopup",
                "PART_ColorPickerToggleButton",
                "PART_RecentColors",
                "PART_StandardColors"
            })
            {
                Assert.IsNotNull(
                    picker.Template.FindName(name, picker),
                    "Missing template part " + name + ".");
            }
        }

        private static void AssertTemplateInstances(ColorCanvas canvas)
        {
            foreach (var name in new[]
            {
                "PART_ColorShadingCanvas",
                "PART_ColorShadeSelector",
                "PART_HexadecimalTextBox",
                "PART_SpectrumSlider"
            })
            {
                Assert.IsNotNull(
                    canvas.Template.FindName(name, canvas),
                    "Missing template part " + name + ".");
            }
        }

        private static void AssertTemplateParts(Type type, params string[] expected)
        {
            var actual = type
                .GetCustomAttributes(typeof(TemplatePartAttribute), true)
                .Cast<TemplatePartAttribute>()
                .Select(attribute => attribute.Name)
                .OrderBy(name => name)
                .ToArray();

            CollectionAssert.AreEqual(
                expected.OrderBy(name => name).ToArray(),
                actual);
        }

        private static string GetPublicKeyToken(
            System.Reflection.AssemblyName assemblyName)
        {
            return BitConverter.ToString(assemblyName.GetPublicKeyToken())
                .Replace("-", string.Empty)
                .ToLowerInvariant();
        }

        private static IEnumerable<T> FindVisualDescendants<T>(
            DependencyObject root)
            where T : DependencyObject
        {
            if (root == null)
                yield break;

            var childCount = VisualTreeHelper.GetChildrenCount(root);
            for (var i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                var match = child as T;
                if (match != null)
                    yield return match;

                foreach (var descendant in FindVisualDescendants<T>(child))
                    yield return descendant;
            }
        }

        private static void EnsurePackUriParser()
        {
            _ = Application.ResourceAssembly;
        }

        private static void DrainDispatcher()
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => frame.Continue = false));
            Dispatcher.PushFrame(frame);
        }

        private sealed class ControlHost : IDisposable
        {
            internal ControlHost(UIElement control)
            {
                Window = new Window
                {
                    Width = 640,
                    Height = 480,
                    Content = control,
                    ShowInTaskbar = false,
                    WindowStyle = WindowStyle.None
                };
                Window.Show();
                Window.Activate();
            }

            internal Window Window { get; }

            public void Dispose()
            {
                Window.Content = null;
                Window.Close();
                DrainDispatcher();
            }
        }

        private sealed class PropertyGridBindingModel
        {
            public object SelectedObject { get; set; }
        }

        private sealed class PropertyGridSample
        {
            public string Text { get; set; }

            [ReadOnly(true)]
            public string ReadOnlyText { get; set; }

            public NestedSample Nested { get; set; }

            public SampleMode? OptionalMode { get; set; }

            public Brush Brush { get; set; }

            public IPAddress Address { get; set; }

            public string[] Array { get; set; }

            public List<string> Items { get; set; }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        private sealed class NestedSample
        {
            public string Name { get; set; }
        }

        private enum SampleMode
        {
            First,
            Second
        }

        private sealed class CollectionItemSample
        {
            public string Name { get; set; }
        }
    }
}
