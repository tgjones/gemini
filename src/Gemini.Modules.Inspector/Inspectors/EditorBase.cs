using System;
using System.ComponentModel;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace Gemini.Modules.Inspector.Inspectors
{
    public abstract class EditorBase<TValue> : InspectorBase, IEditor, IDisposable
    {
        private BoundPropertyDescriptor _boundPropertyDescriptor;

        public EditorBase()
        {
            IsUndoEnabled = true;
        }

        public bool IsUndoEnabled
        {
            get;
            set;
        }

        public bool CanReset
        {
            get
            {
                return BoundPropertyDescriptor.PropertyDescriptor.CanResetValue(BoundPropertyDescriptor.PropertyOwner);
            }
        }

        public void Reset()
        {
            var descriptor = BoundPropertyDescriptor.PropertyDescriptor;
            var owner = BoundPropertyDescriptor.PropertyOwner;

            if (descriptor.CanResetValue(owner))
                descriptor.ResetValue(owner);
        }

        public override string Name
        {
            get { return BoundPropertyDescriptor.PropertyDescriptor.DisplayName; }
        }

        public string Description
        {
            get
            {
                if (!string.IsNullOrEmpty(BoundPropertyDescriptor.PropertyDescriptor.Description))
                    return BoundPropertyDescriptor.PropertyDescriptor.Description;
                return Name;
            }
        }

        private void CleanupPropertyChanged()
        {
            if (_boundPropertyDescriptor != null) {
                if (_boundPropertyDescriptor.PropertyDescriptor.SupportsChangeEvents) {
                    _boundPropertyDescriptor.ValueChanged -= OnValueChanged;
                } else if (typeof(INotifyPropertyChanged).IsAssignableFrom(_boundPropertyDescriptor.PropertyOwner.GetType())) {
                    ((INotifyPropertyChanged)_boundPropertyDescriptor.PropertyOwner).PropertyChanged -= OnPropertyChanged;
                }
            }
        }

        public BoundPropertyDescriptor BoundPropertyDescriptor
        {
            get { return _boundPropertyDescriptor; }
            set
            {
                CleanupPropertyChanged();

                _boundPropertyDescriptor = value;

                if (value.PropertyDescriptor.SupportsChangeEvents) {
                    value.ValueChanged += OnValueChanged;
                } else if (typeof(INotifyPropertyChanged).IsAssignableFrom(value.PropertyOwner.GetType())) {
                    ((INotifyPropertyChanged)value.PropertyOwner).PropertyChanged += OnPropertyChanged;
                }
            }
        }

        public override bool IsReadOnly
        {
            get { return BoundPropertyDescriptor.PropertyDescriptor.IsReadOnly; }
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            NotifyOfPropertyChange(() => Value);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(BoundPropertyDescriptor.PropertyDescriptor.Name))
                NotifyOfPropertyChange(() => Value);
        }

        public TValue Value
        {
            get { return (TValue) BoundPropertyDescriptor.Value; }
            set
            {
                if (Equals(Value, value))
                    return;

                if (IsUndoEnabled)
                {
                    IoC.Get<IShell>().ActiveItem.UndoRedoManager.ExecuteAction(
                        new ChangeObjectValueAction(BoundPropertyDescriptor, value));
                }
                else
                {
                    BoundPropertyDescriptor.Value = value;
                }

                NotifyOfPropertyChange(() => Value);
            }
        }

        public virtual void Dispose()
        {
            CleanupPropertyChanged();
        }
    }
}