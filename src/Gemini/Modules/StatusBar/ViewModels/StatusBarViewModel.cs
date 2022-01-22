using System;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;

namespace Gemini.Modules.StatusBar.ViewModels
{
	[Export(typeof(IStatusBar))]
	public class StatusBarViewModel : PropertyChangedBase, IStatusBar, IViewAware
    {
        private readonly StatusBarItemCollection _items;
        private IStatusBarView _statusBarView;

        public event EventHandler<ViewAttachedEventArgs> ViewAttached;

        public IObservableCollection<StatusBarItemViewModel> Items
	    {
            get { return _items; }
	    }

	    public StatusBarViewModel()
        {
            _items = new StatusBarItemCollection();
            _items.CollectionChanged += OnItemsCollectionChanged;
        }

        private void OnItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _statusBarView?.RefreshGridColumns();
        }

        public void AddItem(string message, GridLength width)
	    {
	        Items.Add(new StatusBarItemViewModel(message, width));
        }

        public void AttachView(object view, object context = null)
        {
            if (view is IStatusBarView statusBarView)
            {
                ViewAttached?.Invoke(this, new ViewAttachedEventArgs()
                {
                    Context = context,
                    View = statusBarView
                });
                _statusBarView = view as IStatusBarView;
            }
        }

        public object GetView(object context = null) => _statusBarView;

        private class StatusBarItemCollection : BindableCollection<StatusBarItemViewModel>
        {
            protected override void InsertItemBase(int index, StatusBarItemViewModel item)
            {
                item.Index = index;
                base.InsertItemBase(index, item);
            }

            protected override void SetItemBase(int index, StatusBarItemViewModel item)
            {
                item.Index = index;
                base.SetItemBase(index, item);
            }
        }
	}
}
