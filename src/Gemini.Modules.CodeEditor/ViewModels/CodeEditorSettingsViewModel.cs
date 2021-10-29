using Caliburn.Micro;
using Gemini.Framework.Services;
using Gemini.Modules.Settings;
using System.ComponentModel.Composition;
using System.Linq;

namespace Gemini.Modules.CodeEditor.ViewModels
{
    [Export(typeof(ISettingsEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CodeEditorSettingsViewModel : PropertyChangedBase, ISettingsEditor
    {
        private readonly IShell _shell;

        private bool _showLineNumbers;
        private bool _showSpaces;
        private bool _showTabs;
        private bool _showEndOfLine;
        private bool _wordWrap;

        public bool ShowLineNumbers
        {
            get => _showLineNumbers;
            set => Set(ref _showLineNumbers, value);
        }

        public bool ShowSpaces
        {
            get => _showSpaces;
            set => Set(ref _showSpaces, value);
        }

        public bool ShowTabs
        {
            get => _showTabs;
            set => Set(ref _showTabs, value);
        }

        public bool ShowEndOfLine
        {
            get => _showEndOfLine;
            set => Set(ref _showEndOfLine, value);
        }

        public bool WordWrap
        {
            get => _wordWrap;
            set => Set(ref _wordWrap, value);
        }

        public string SettingsPageName
        {
            get { return Properties.Resources.SettingsPageGeneral; }
        }

        public string SettingsPagePath
        {
            get { return Properties.Resources.SettingsPageCodeEditor; }
        }

        public void ApplyChanges()
        {
            Properties.Settings.Default.ShowLineNumbers = ShowLineNumbers;
            Properties.Settings.Default.ShowSpaces = ShowSpaces;
            Properties.Settings.Default.ShowTabs = ShowTabs;
            Properties.Settings.Default.ShowEndOfLine = ShowEndOfLine;
            Properties.Settings.Default.WordWrap = WordWrap;
            Properties.Settings.Default.Save();
            ApplySettingsToDocuments();
        }

        private void ApplySettingsToDocuments()
        {
            var editors = _shell.Documents.OfType<CodeEditorViewModel>().ToList();
            foreach (var editor in editors)
            {
                editor.ApplySettings();
            }
        }

        [ImportingConstructor]
        public CodeEditorSettingsViewModel([Import]IShell shell)
        {
            _shell = shell;
            ShowLineNumbers = Properties.Settings.Default.ShowLineNumbers;
            ShowSpaces = Properties.Settings.Default.ShowSpaces;
            ShowTabs = Properties.Settings.Default.ShowTabs;
            ShowEndOfLine = Properties.Settings.Default.ShowEndOfLine;
            WordWrap = Properties.Settings.Default.WordWrap;
        }
    }
}
