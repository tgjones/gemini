using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Caliburn.Micro;

namespace Gemini.Framework.Collections
{
    // Based on SO answer: http://stackoverflow.com/a/6245230/208817
    public class ObservableStack<T> : Stack<T>, INotifyCollectionChanged, INotifyPropertyChanged, IObservableCollection<T>
    {
        public ObservableStack()
        {
        }

        public ObservableStack(IEnumerable<T> collection)
        {
            foreach (var item in collection)
                base.Push(item);
        }

        public ObservableStack(List<T> list)
        {
            foreach (var item in list)
                base.Push(item);
        }

        public new virtual void Clear()
        {
            base.Clear();
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new virtual T Pop()
        {
            var item = base.Pop();
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return item;
        }

        public new virtual void Push(T item)
        {
            base.Push(item);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;


        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.RaiseCollectionChanged(e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(e);
        }


        protected event PropertyChangedEventHandler PropertyChanged;


        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
                this.CollectionChanged(this, e);
        }

        private void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, e);
        }


        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { this.PropertyChanged += value; }
            remove { this.PropertyChanged -= value; }
        }

        void ICollection<T>.Add(T item)
        {
            throw new System.NotSupportedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new System.NotSupportedException();
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        int IList<T>.IndexOf(T item)
        {
            throw new System.NotSupportedException();
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new System.NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new System.NotSupportedException();
        }

        T IList<T>.this[int index]
        {
            get { throw new System.NotSupportedException(); }
            set { throw new System.NotSupportedException(); }
        }

        void INotifyPropertyChangedEx.NotifyOfPropertyChange(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        void INotifyPropertyChangedEx.Refresh()
        {
            throw new System.NotSupportedException();
        }

        bool INotifyPropertyChangedEx.IsNotifying
        {
            get { throw new System.NotSupportedException(); }
            set { throw new System.NotSupportedException(); }
        }

        void IObservableCollection<T>.AddRange(IEnumerable<T> items)
        {
            throw new System.NotSupportedException();
        }

        void IObservableCollection<T>.RemoveRange(IEnumerable<T> items)
        {
            throw new System.NotSupportedException();
        }
    }
}