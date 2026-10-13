using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Search;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeEditorControl = Gemini.Modules.CodeEditor.Controls.CodeEditor;

namespace Gemini.Tests.Modules.CodeEditor
{
    [STATestClass]
    [DoNotParallelize]
    public sealed class CodeEditorSearchPanelTests
    {
        [TestMethod]
        [Timeout(10000)]
        public void ConstructionLoadAndDocumentReplacement_OwnOneSearchPanel()
        {
            var baselineEditor = new TextEditor();
            var editor = new CodeEditorControl();
            var baselineHandlerCount =
                baselineEditor.TextArea.DefaultInputHandler.NestedInputHandlers.Count;

            Assert.AreEqual(
                baselineHandlerCount + 1,
                editor.TextArea.DefaultInputHandler.NestedInputHandlers.Count);

            using (var host = new EditorHost(editor))
            {
                DrainDispatcher();
                var installedHandlerCount =
                    editor.TextArea.DefaultInputHandler.NestedInputHandlers.Count;

                for (var i = 0; i < 3; i++)
                {
                    host.Window.Content = null;
                    DrainDispatcher();
                    host.Window.Content = editor;
                    DrainDispatcher();
                    editor.Document = new TextDocument("document " + i);
                }

                Assert.AreEqual(
                    installedHandlerCount,
                    editor.TextArea.DefaultInputHandler.NestedInputHandlers.Count);

                ApplicationCommands.Find.Execute(null, editor.TextArea);
                DrainDispatcher();

                Assert.AreEqual(1, GetSearchPanels(editor).Count);
            }
        }

        [TestMethod]
        [Timeout(10000)]
        public void SearchPanel_DocumentReplacementAndOptions_TargetCurrentDocument()
        {
            var editor = new CodeEditorControl();
            using (var host = new EditorHost(editor))
            {
                DrainDispatcher();
                ApplicationCommands.Find.Execute(null, editor.TextArea);
                DrainDispatcher();

                var panel = Assert.ContainsSingle(GetSearchPanels(editor));

                AssertSearch(
                    editor,
                    panel,
                    "Alpha alpha",
                    "alpha",
                    matchCase: true,
                    wholeWords: false,
                    useRegex: false,
                    expectedSelection: "alpha");
                AssertSearch(
                    editor,
                    panel,
                    "cat scatter cat",
                    "cat",
                    matchCase: false,
                    wholeWords: true,
                    useRegex: false,
                    expectedSelection: "cat");
                AssertSearch(
                    editor,
                    panel,
                    "item1 item22",
                    "\\d+",
                    matchCase: false,
                    wholeWords: false,
                    useRegex: true,
                    expectedSelection: "1");
                AssertSearch(
                    editor,
                    panel,
                    "no matching text",
                    "absent",
                    matchCase: false,
                    wholeWords: false,
                    useRegex: false,
                    expectedSelection: string.Empty);

                panel.SearchPattern = string.Empty;
                panel.UseRegex = true;

                var searchTextBox = FindVisualDescendants<TextBox>(panel)
                    .Single(x => x.Name == "PART_searchTextBox");
                searchTextBox.Text = "[";
                DrainDispatcher();
                Assert.IsTrue(Validation.GetHasError(searchTextBox));

                panel.Close();
                Assert.IsTrue(panel.IsClosed);
            }
        }

        [TestMethod]
        public void DocumentReplacement_ReleasesOldDocument()
        {
            var editor = new CodeEditorControl();
            var oldDocument = ReplaceDocument(editor);

            CollectGarbage();

            Assert.IsFalse(oldDocument.IsAlive);
        }

        [TestMethod]
        [Timeout(10000)]
        public void ClosedHost_ReleasesCodeEditorWithOwnedSubscriptions()
        {
            var editor = CreateReleasedEditor();

            CollectGarbage();

            Assert.IsFalse(editor.IsAlive);
        }

        private static void AssertSearch(
            CodeEditorControl editor,
            SearchPanel panel,
            string text,
            string pattern,
            bool matchCase,
            bool wholeWords,
            bool useRegex,
            string expectedSelection)
        {
            editor.Document = new TextDocument(text);
            editor.CaretOffset = 0;
            editor.Select(0, 0);
            panel.SearchPattern = string.Empty;
            panel.MatchCase = matchCase;
            panel.WholeWords = wholeWords;
            panel.UseRegex = useRegex;
            panel.SearchPattern = pattern;

            Assert.AreEqual(expectedSelection, editor.SelectedText);
        }

        private static List<SearchPanel> GetSearchPanels(CodeEditorControl editor)
        {
            var layer = AdornerLayer.GetAdornerLayer(editor.TextArea);
            if (layer == null)
                return new List<SearchPanel>();

            var adorners = layer.GetAdorners(editor.TextArea);
            if (adorners == null)
                return new List<SearchPanel>();

            return adorners
                .SelectMany(FindVisualDescendants<SearchPanel>)
                .ToList();
        }

        private static IEnumerable<T> FindVisualDescendants<T>(
            DependencyObject root)
            where T : DependencyObject
        {
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

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static WeakReference ReplaceDocument(CodeEditorControl editor)
        {
            var document = new TextDocument("old document");
            editor.Document = document;
            var reference = new WeakReference(document);
            editor.Document = new TextDocument("new document");
            return reference;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static WeakReference CreateReleasedEditor()
        {
            var editor = new CodeEditorControl();
            using (var host = new EditorHost(editor))
            {
                DrainDispatcher();
                editor.Document = new TextDocument("released editor");
                Keyboard.ClearFocus();
                host.Window.Content = null;
                host.Window.UpdateLayout();
            }

            DrainDispatcher();
            return new WeakReference(editor);
        }

        private static void DrainDispatcher()
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => frame.Continue = false));
            Dispatcher.PushFrame(frame);
        }

        private static void CollectGarbage()
        {
            for (var i = 0; i < 3; i++)
            {
                CommandManager.InvalidateRequerySuggested();
                DrainDispatcher();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private sealed class EditorHost : IDisposable
        {
            internal EditorHost(UIElement editor)
            {
                Window = new Window
                {
                    Width = 640,
                    Height = 480,
                    Content = editor,
                    ShowInTaskbar = false,
                    WindowStyle = WindowStyle.None
                };
                Window.Show();
            }

            internal Window Window { get; }

            public void Dispose()
            {
                Window.Content = null;
                Window.Close();
            }
        }
    }
}
