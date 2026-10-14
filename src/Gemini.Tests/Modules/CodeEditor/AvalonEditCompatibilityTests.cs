using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Gemini.Modules.CodeEditor;
using Gemini.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeEditorControl = Gemini.Modules.CodeEditor.Controls.CodeEditor;

namespace Gemini.Tests.Modules.CodeEditor
{
    [STATestClass]
    [DoNotParallelize]
    public sealed class AvalonEditCompatibilityTests
    {
        private static readonly string[] SettingNames =
        {
            "ShowEndOfLine",
            "ShowLineNumbers",
            "ShowSpaces",
            "ShowTabs",
            "WordWrap"
        };

        [TestMethod]
        public void ApplySettings_PreservesAllConfiguredEditorOptions()
        {
            var settings = GetSettings();
            var originalValues = SettingNames.ToDictionary(
                name => name,
                name => GetSetting(settings, name));

            try
            {
                SetSettings(settings, true);
                var editor = new CodeEditorControl();

                Assert.IsTrue(editor.Options.ShowEndOfLine);
                Assert.IsTrue(editor.ShowLineNumbers);
                Assert.IsTrue(editor.Options.ShowSpaces);
                Assert.IsTrue(editor.Options.ShowTabs);
                Assert.IsTrue(editor.WordWrap);
                Assert.AreEqual("Consolas", editor.FontFamily.Source);
                Assert.AreEqual(12d, editor.FontSize);

                SetSettings(settings, false);
                editor.ApplySettings();

                Assert.IsFalse(editor.Options.ShowEndOfLine);
                Assert.IsFalse(editor.ShowLineNumbers);
                Assert.IsFalse(editor.Options.ShowSpaces);
                Assert.IsFalse(editor.Options.ShowTabs);
                Assert.IsFalse(editor.WordWrap);
            }
            finally
            {
                foreach (var originalValue in originalValues)
                    SetSetting(settings, originalValue.Key, originalValue.Value);
            }
        }

        [TestMethod]
        public void LanguageDefinitions_BuiltInsAndImportedOverrideRemainUsable()
        {
            using (CreateIoCScope(Enumerable.Empty<ILanguageDefinition>()))
            {
                var manager = new LanguageDefinitionManager();

                AssertLanguage(manager, ".cs", "C#");
                AssertLanguage(manager, ".xml", "XML");
                AssertLanguage(manager, ".md", "MarkDown");
            }

            var customHighlighting = HighlightingManager.Instance.GetDefinition("XML");
            var custom = new TestLanguageDefinition(
                "C#",
                new[] { ".external-cs" },
                customHighlighting);
            using (CreateIoCScope(new[] { custom }))
            {
                var manager = new LanguageDefinitionManager();

                Assert.AreSame(
                    custom,
                    manager.GetDefinitionByExtension(".external-cs"));
                Assert.AreSame(
                    customHighlighting,
                    manager.GetDefinitionByExtension(".external-cs")
                        .SyntaxHighlighting);
            }
        }

        [TestMethod]
        public void TextDocument_GroupedUndoRedoAndReplacementRemainIndependent()
        {
            var originalDocument = new TextDocument("root");
            originalDocument.BeginUpdate();
            originalDocument.Insert(originalDocument.TextLength, " one");
            originalDocument.Insert(originalDocument.TextLength, " two");
            originalDocument.EndUpdate();

            Assert.AreEqual("root one two", originalDocument.Text);
            Assert.IsTrue(originalDocument.UndoStack.CanUndo);

            originalDocument.UndoStack.Undo();
            Assert.AreEqual("root", originalDocument.Text);
            Assert.IsTrue(originalDocument.UndoStack.CanRedo);

            originalDocument.UndoStack.Redo();
            Assert.AreEqual("root one two", originalDocument.Text);

            originalDocument.UndoStack.Undo();
            originalDocument.Insert(originalDocument.TextLength, " replacement");
            Assert.IsFalse(originalDocument.UndoStack.CanRedo);

            var editor = new CodeEditorControl
            {
                Document = originalDocument
            };
            var replacementDocument = new TextDocument("new document");
            editor.Document = replacementDocument;
            originalDocument.Insert(originalDocument.TextLength, " detached");

            Assert.AreSame(replacementDocument, editor.Document);
            Assert.AreEqual("new document", editor.Text);
            Assert.IsTrue(originalDocument.Text.EndsWith(
                " detached",
                StringComparison.Ordinal));
        }

        private static void AssertLanguage(
            LanguageDefinitionManager manager,
            string extension,
            string expectedName)
        {
            var definition = manager.GetDefinitionByExtension(extension);

            Assert.IsNotNull(definition);
            Assert.AreEqual(expectedName, definition.Name);
            Assert.IsNotNull(definition.SyntaxHighlighting);
        }

        private static object GetSettings()
        {
            var settingsType = typeof(CodeEditorControl).Assembly.GetType(
                "Gemini.Modules.CodeEditor.Properties.Settings",
                throwOnError: true);
            return settingsType.GetProperty(
                    "Default",
                    BindingFlags.Public | BindingFlags.Static)
                .GetValue(null);
        }

        private static bool GetSetting(object settings, string name)
        {
            return (bool)settings.GetType().GetProperty(name).GetValue(settings);
        }

        private static void SetSettings(object settings, bool value)
        {
            foreach (var settingName in SettingNames)
                SetSetting(settings, settingName, value);
        }

        private static void SetSetting(
            object settings,
            string name,
            bool value)
        {
            settings.GetType().GetProperty(name).SetValue(settings, value);
        }

        private static IoCOverrideScope CreateIoCScope(
            IEnumerable<ILanguageDefinition> languageDefinitions)
        {
            var definitions = languageDefinitions.Cast<object>().ToArray();
            return new IoCOverrideScope(
                (type, key) => null,
                type => type == typeof(ILanguageDefinition)
                    ? definitions
                    : Enumerable.Empty<object>(),
                instance => { });
        }

        private sealed class TestLanguageDefinition : ILanguageDefinition
        {
            internal TestLanguageDefinition(
                string name,
                IEnumerable<string> fileExtensions,
                IHighlightingDefinition syntaxHighlighting)
            {
                Name = name;
                FileExtensions = fileExtensions;
                SyntaxHighlighting = syntaxHighlighting;
            }

            public string Name { get; }

            public IEnumerable<string> FileExtensions { get; set; }

            public IHighlightingDefinition SyntaxHighlighting { get; }

            public string CustomSyntaxHighlightingFileName { get; set; }
        }
    }
}
