using System.IO;
using System.Threading.Tasks;

namespace Gemini.Framework
{
    public abstract class PersistedDocument : Document, IPersistedDocument
    {
        private bool _isDirty;

        public bool IsNew { get; private set; }
        public string FileName { get; private set; }
        public string FilePath { get; private set; }

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

        public override void CanClose(System.Action<bool> callback)
        {
            // TODO: Show save prompt.
            callback(!IsDirty);
        }

        private void UpdateDisplayName()
        {
            DisplayName = (IsDirty) ? FileName + "*" : FileName;
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
    }
}