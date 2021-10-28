using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Threading;
using Gemini.Modules.CodeEditor.Commands;
using Gemini.Modules.CodeEditor.Views;

namespace Gemini.Modules.CodeEditor.ViewModels
{
    [Export(typeof(CodeEditorViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
#pragma warning disable 659
    public class CodeEditorViewModel : PersistedDocument,
        ICommandHandler<ShowLineNumbersCommandDefinition>,
        ICommandHandler<ShowEndOfLineCommandDefinition>,
        ICommandHandler<ShowSpacesCommandDefinition>,
        ICommandHandler<ShowTabsCommandDefinition>,
        ICommandHandler<WordWrapCommandDefinition>
#pragma warning restore 659
    {
        private readonly LanguageDefinitionManager _languageDefinitionManager;
        private bool _delayedViewLoaded;
        private string _originalText;
        private ICodeEditorView _view;

        [ImportingConstructor]
        public CodeEditorViewModel(LanguageDefinitionManager languageDefinitionManager)
        {
            _languageDefinitionManager = languageDefinitionManager;
        }

        public override bool ShouldReopenOnStart
        {
            get { return true; }
        }

        public override void SaveState(BinaryWriter writer)
        {
            writer.Write(FilePath);
        }

        public override void LoadState(BinaryReader reader)
        {
            Load(reader.ReadString());
        }

        protected override void OnViewLoaded(object view)
        {
            _view = (ICodeEditorView) view;
            if (_delayedViewLoaded)
            {
                ApplyOriginalText();
                _delayedViewLoaded = false;
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as CodeEditorViewModel;
            return other != null
                && string.Equals(FilePath, other.FilePath, StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(FileName, other.FileName, StringComparison.InvariantCultureIgnoreCase);
        }

        protected override Task DoNew()
        {
            _originalText = string.Empty;
            ApplyOriginalText();
            return TaskUtility.Completed;
        }

        protected override Task DoLoad(string filePath)
        {
            _originalText = File.ReadAllText(filePath);
            ApplyOriginalText();
            return TaskUtility.Completed;
        }

        protected override Task DoSave(string filePath)
        {
            var newText = _view.TextEditor.Text;
            File.WriteAllText(filePath, newText);
            _originalText = newText;
            return TaskUtility.Completed;
        }

        private void ApplyOriginalText()
        {
            if (_view == null)
            {
                _delayedViewLoaded = true;
                return;
            }

            _view.TextEditor.Text = _originalText;

            _view.TextEditor.TextChanged += delegate
            {
                IsDirty = string.Compare(_originalText, _view.TextEditor.Text) != 0;
            };

            var fileExtension = Path.GetExtension(FileName).ToLower();

            ILanguageDefinition languageDefinition = _languageDefinitionManager.GetDefinitionByExtension(fileExtension);

            SetLanguage(languageDefinition);
        }

        private void SetLanguage(ILanguageDefinition languageDefinition)
        {
            _view.TextEditor.SyntaxHighlighting = (languageDefinition != null)
                ? languageDefinition.SyntaxHighlighting
                : null;
        }

        public void ApplySettings()
        {
            _view?.ApplySettings();
        }

        #region CommandHandlers
        void ICommandHandler<WordWrapCommandDefinition>.Update(Command command)
        {
            command.Enabled = _view?.TextEditor != null;
            command.Checked = _view?.TextEditor.WordWrap ?? false;
        }

        Task ICommandHandler<WordWrapCommandDefinition>.Run(Command command)
        {
            _view.TextEditor.WordWrap = !_view.TextEditor.WordWrap;
            command.Checked = _view?.TextEditor.WordWrap ?? false;
            return TaskUtility.Completed;
        }

        void ICommandHandler<ShowLineNumbersCommandDefinition>.Update(Command command)
        {
            command.Enabled = _view?.TextEditor != null;
            command.Checked = _view?.TextEditor.ShowLineNumbers ?? false;
        }

        Task ICommandHandler<ShowLineNumbersCommandDefinition>.Run(Command command)
        {
            _view.TextEditor.ShowLineNumbers = !_view.TextEditor.ShowLineNumbers;
            command.Checked = _view.TextEditor.ShowLineNumbers;
            Properties.Settings.Default.ShowLineNumbers = _view.TextEditor.ShowLineNumbers;
            Properties.Settings.Default.Save();
            return TaskUtility.Completed;
        }

        void ICommandHandler<ShowSpacesCommandDefinition>.Update(Command command)
        {
            command.Enabled = _view?.TextEditor != null;
            command.Checked = _view?.TextEditor.Options.ShowSpaces ?? false;
        }

        Task ICommandHandler<ShowSpacesCommandDefinition>.Run(Command command)
        {
            _view.TextEditor.Options.ShowSpaces = !_view.TextEditor.Options.ShowSpaces;
            command.Checked = _view.TextEditor.Options.ShowSpaces;
            Properties.Settings.Default.ShowSpaces = _view.TextEditor.Options.ShowSpaces;
            Properties.Settings.Default.Save();
            return TaskUtility.Completed;
        }

        void ICommandHandler<ShowEndOfLineCommandDefinition>.Update(Command command)
        {
            command.Enabled = _view?.TextEditor != null;
            command.Checked = _view?.TextEditor.Options.ShowEndOfLine ?? false;
        }

        Task ICommandHandler<ShowEndOfLineCommandDefinition>.Run(Command command)
        {
            _view.TextEditor.Options.ShowEndOfLine = !_view.TextEditor.Options.ShowEndOfLine;
            command.Checked = _view.TextEditor.Options.ShowEndOfLine;
            Properties.Settings.Default.ShowEndOfLine = _view.TextEditor.Options.ShowEndOfLine;
            Properties.Settings.Default.Save();
            return TaskUtility.Completed;
        }

        void ICommandHandler<ShowTabsCommandDefinition>.Update(Command command)
        {
            command.Enabled = _view?.TextEditor != null;
            command.Checked = _view?.TextEditor.Options.ShowTabs ?? false;
        }

        Task ICommandHandler<ShowTabsCommandDefinition>.Run(Command command)
        {
            _view.TextEditor.Options.ShowTabs = !_view.TextEditor.Options.ShowTabs;
            command.Checked = _view.TextEditor.Options.ShowTabs;
            Properties.Settings.Default.ShowTabs = _view.TextEditor.Options.ShowTabs;
            Properties.Settings.Default.Save();
            return TaskUtility.Completed;
        }
        #endregion
    }
}
