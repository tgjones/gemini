using Caliburn.Micro;
using Gemini.Framework.Services;
using Gemini.Properties;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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

        // ShouldReopenOnStart, SaveState and LoadState are default methods of PersistedDocument.
        public override bool ShouldReopenOnStart
        {
            get { return (FilePath != null); }  // if FilePath is null, SaveState() will generate an NullExceptionError
        }
                
        public override void SaveState(BinaryWriter writer)
        {
            writer.Write(FilePath);
        }

        public override async void LoadState(BinaryReader reader)
        {
            await Load(reader.ReadString());
        }

        public override async void CanClose(System.Action<bool> callback)
        {
            if (IsDirty)
            {
                // Show save prompt.  
                // Note that CanClose method of Demo ShellViewModel blocks this. 
                string title = IoC.Get<IMainWindow>().Title;
                string fileName = Path.GetFileNameWithoutExtension(FileName);
                string fileExtension = Path.GetExtension(FileName);
                var fileType = IoC.GetAll<IEditorProvider>()
                                  .SelectMany(x => x.FileTypes)
                                  .SingleOrDefault(x => x.FileExtension == fileExtension);

                string message = string.Format(Resources.SaveChangesBeforeClosingMessage, fileType.Name, fileName);
                var result = MessageBox.Show(message, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    if (IsNew)
                    {
                        // Ask new file path.
                        var filter = string.Empty;
                        if (fileType != null)
                            filter = fileType.Name + "|*" + fileType.FileExtension + "|";
                        filter += Properties.Resources.AllFiles + "|*.*";

                        var dialog = new SaveFileDialog() { FileName = this.FileName, Filter = filter };
                        if (dialog.ShowDialog() == true)
                        {
                            // Save file.
                            await Save(dialog.FileName);

                            // Add to recent files. Temporally, commented out.
                            //IShell _shell = IoC.Get<IShell>();
                            //_shell.RecentFiles.Update(dialog.FileName);
                        }
                        else
                        {
                            callback(false);
                            return;
                        }
                    }
                    else
                    {
                        // Save file.
                        await Save(FilePath);
                    }
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    callback(false);
                    return;
                }
            }

            callback(true);
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