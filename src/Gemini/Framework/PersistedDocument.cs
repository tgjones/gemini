using System;
using System.IO;
using System.Threading.Tasks;

namespace Gemini.Framework
{
    public abstract class PersistedDocument : Document, IPersistedDocument
    {
        private bool _isDirty;
        private string _documentName;

        public bool IsNew { get; private set; }

        public string DocumentName
        {
            get
            {
                return _documentName;
            }
            private set
            {
                if (value == _documentName)
                    return;
                _documentName = value;
                NotifyOfPropertyChange(() => DocumentName);
                NotifyOfPropertyChange(() => DisplayName);
            }
        }

        public string DocumentPath { get; private set; }
        public abstract DocumentType DocumentType { get; }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (value == _isDirty)
                    return;
                _isDirty = value;
                NotifyOfPropertyChange(() => IsDirty);
                NotifyOfPropertyChange(() => DisplayName);
            }
        }

        public override string DisplayName
        {
            get
            {
                return IsDirty ? DocumentName + "*" : DocumentName;
            }
        }
        
        public override void CanClose(Action<bool> callback)
        {
            // TODO: Show save prompt.
            callback(!IsDirty);
        }
        
        public async Task New(string documentName)
        {
            DocumentName = documentName;
            IsNew = true;
            IsDirty = false;
            await DoNew();
        }

        protected abstract Task DoNew();

        public async Task Load(string documentPath)
        {
            DocumentPath = documentPath;
            DocumentName = GetName(documentPath);
            IsNew = false;
            IsDirty = false;
            await DoLoad(documentPath);
        }

        protected abstract Task DoLoad(string documentPath);

        public async Task Save(string documentPath)
        {
            DocumentPath = documentPath;
            DocumentName = GetName(documentPath);
            await DoSave(documentPath);
            IsDirty = false;
            IsNew = false;
        }

        protected abstract Task DoSave(string documentPath);

        public string GetName(string path)
        {
            switch (DocumentType)
            {
                case DocumentType.Folder:
                    return Path.GetFileName(Path.GetDirectoryName(path));
                case DocumentType.File:
                    return Path.GetFileName(path);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}