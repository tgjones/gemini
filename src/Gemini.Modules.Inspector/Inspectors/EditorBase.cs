using System;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace Gemini.Modules.Inspector.Inspectors
{
    public abstract class EditorBase<TValue> : InspectorBase, IEditor, IDisposable
    {
        private BoundPropertyDescriptor _boundPropertyDescriptor;

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

        public BoundPropertyDescriptor BoundPropertyDescriptor
        {
            get { return _boundPropertyDescriptor; }
            set
            {
                if (_boundPropertyDescriptor != null)
                    _boundPropertyDescriptor.ValueChanged -= OnValueChanged;
                _boundPropertyDescriptor = value;
                value.ValueChanged += OnValueChanged;
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

        public TValue Value
        {
            get { return (TValue) BoundPropertyDescriptor.Value; }
            set
            {
                IoC.Get<IShell>().ActiveItem.UndoRedoManager.ExecuteAction(
                    new ChangeObjectValueAction(BoundPropertyDescriptor, value));
                NotifyOfPropertyChange(() => Value);
            }
        }

        public void Dispose()
        {
            if (_boundPropertyDescriptor != null)
                _boundPropertyDescriptor.ValueChanged -= OnValueChanged;
        }
    }
}