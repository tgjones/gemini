using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Gemini.Framework.Commands;
using Gemini.Modules.Shell.Commands;
using Gemini.Properties;

namespace Gemini.Framework
{
    public abstract class PersistedDocument : Document, IPersistedDocument
    {
        private bool _isDirty;

        public bool IsNew { get; private set; }
        public string FileName { get; private set; }

        private string _filePath = null;
        public string FilePath
        {
            get { return _filePath; }
            private set
            {
                _filePath = value;
                NotifyOfPropertyChange(() => FilePath);
                UpdateToolTip();
            }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (value == _isDirty)
                    return;

                _isDirty = value;
                NotifyOfPropertyChange(() => IsDirty);
                UpdateDisplayName();
            }
        }

        public override async Task<bool> CanCloseAsync(CancellationToken cancellationToken)
        {
            if (IsDirty)
            {
                var response = MessageBox.Show(string.Format(Resources.DocumentDirtyCloseConfirmText, FileName), Resources.DocumentDirtyCloseConfirmTitle, MessageBoxButton.YesNoCancel);
                if (response == MessageBoxResult.Cancel)
                {
                    return false;
                }
                if (response == MessageBoxResult.Yes)
                {
                    await Save();
                    return !IsNew; // User may discard file picker prompt
                }
            }
            return true;
        }

        private void UpdateDisplayName()
        {
            DisplayName = IsDirty ? FileName + "*" : FileName;
        }

        private void UpdateToolTip()
        {
            ToolTip = FilePath;
        }

        public async Task New(string fileName)
        {
            FileName = fileName;
            UpdateDisplayName();

            IsNew = true;
            IsDirty = false;

            await DoNew();
        }

        protected abstract Task DoNew();

        public async Task Load(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            UpdateDisplayName();

            IsNew = false;
            IsDirty = false;

            await DoLoad(filePath);
        }

        protected abstract Task DoLoad(string filePath);

        public async Task Save(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            UpdateDisplayName();

            await DoSave(filePath);

            IsDirty = false;
            IsNew = false;
        }

        protected abstract Task DoSave(string filePath);

        public Task Save()
        {
            return ((ICommandHandler<SaveFileCommandDefinition>)this).Run(null);
        }
    }
}
