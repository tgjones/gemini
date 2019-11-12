using Caliburn.Micro;
using System.Collections.Specialized;
using System.ComponentModel.Composition;

namespace Gemini.Modules.RecentFiles.ViewModels
{
    [Export(typeof(IRecentFiles))]
    public class RecentFileViewModel : PropertyChangedBase, IRecentFiles
    {
        private readonly RecentFileItemCollection _items;
        public IObservableCollection<RecentFileItemViewModel> Items
        {
            get { return _items; }
        }

        private int _maxItems = 10;
        public int MaxItems
        {
            get { return _maxItems; }
            set
            {
                _maxItems = value;
                _items.MaxCollectionSize = _maxItems;
            }
        }

        public RecentFileViewModel()
        {
            _items = new RecentFileItemCollection();
            InitializeList();
        }

        /// <summary>
        /// Adds or moves the file to the top of the list.
        /// </summary>
        /// <param name="filePath"></param>
        public void Update(string filePath)
        {
            RecentFileItemViewModel item = new RecentFileItemViewModel(filePath);

            int i = IndexOf(item);
            if (i >= 0)
            {
                if (_items[i].Pinned) return; // do not move pinned items
                RemoveAt(i);
            }

            Insert(0, item);
            SaveList();
        }

        private int IndexOf(string filePath)
        {
            for (int i = 0; i < _items.Count; ++i)
            {
                if (string.Equals(_items[i].FilePath, filePath, System.StringComparison.OrdinalIgnoreCase)) return i;
            }
            return -1;
        }

        private int IndexOf(RecentFileItemViewModel item)
        {
            return IndexOf(item.FilePath);
        }

        public void RemoveItem(string filePath)
        {
            RecentFileItemViewModel item = new RecentFileItemViewModel(filePath);

            int i = IndexOf(item);
            if (i >= 0)
            {
                _items.RemoveAt(i);
                SaveList();
            }
        }

        private void RemoveAt(int i)
        {
            _items.RemoveAt(i);
        }

        private void Insert(int i, RecentFileItemViewModel item)
        {
            _items.Insert(i, item);
        }

        private void InitializeList()
        {
            // Create a new collection if it was not serialized before.
            if (Properties.Settings.Default.RecentDocuments == null)
            {
                Properties.Settings.Default.RecentDocuments = new StringCollection();
            }

            foreach (string filePath in Properties.Settings.Default.RecentDocuments)
            {
                _items.Add(new RecentFileItemViewModel(filePath));
            }
        }

        private void SaveList()
        {
            StringCollection docs = Properties.Settings.Default.RecentDocuments = new StringCollection();

            foreach (var item in _items)
            {
                docs.Add(item.FilePath);
            }

            Properties.Settings.Default.Save();
        }

        private class RecentFileItemCollection : BindableCollection<RecentFileItemViewModel>
        {
            public int MaxCollectionSize { get; set; }

            public RecentFileItemCollection(int maxCollectionSize = 10)
                : base()
            {
                MaxCollectionSize = maxCollectionSize;
            }

            protected override void InsertItemBase(int index, RecentFileItemViewModel item)
            {
                item.Index = index;
                base.InsertItemBase(index, item);

                if (MaxCollectionSize > 0 && MaxCollectionSize < Count)
                {
                    for (int i = Count - 1; i >= MaxCollectionSize; i--)
                    {
                        RemoveAt(i);
                    }
                }
            }

            protected override void SetItemBase(int index, RecentFileItemViewModel item)
            {
                item.Index = index;
                base.SetItemBase(index, item);
            }
        }
    }
}