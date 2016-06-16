using System;

using Caliburn.Micro;

using Gemini.Framework.Services;

namespace Gemini.Modules.Inspector.Inspectors
{
    /// <summary>
    /// This class is used for values that should only be updated after the
    /// user has finished editing them. The view needs to call OnBeginEdit when
    /// the user has started editing to capture the current value and call
    /// OnEndEdit to commit the old and new value to the undo / redo manager.
    /// </summary>
    /// <typeparam name="TValue">Type of the value</typeparam>
    public abstract class SelectiveUndoEditorBase<TValue> : EditorBase<TValue>, IDisposable
    {
        private bool _undoEnabled = true;

        public override TValue Value
        {
            get { return (TValue)BoundPropertyDescriptor.Value; }
            set {
                if (Equals(Value, value))
                    return;

                if (_undoEnabled)
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

        private object _originalValue = null;

        protected void OnBeginEdit()
        {
            _undoEnabled = false;
            _originalValue = Value;
        }

        protected void OnEndEdit()
        {
            if (_originalValue == null)
                return;

            try
            {
                var value = Value;
                if (!_originalValue.Equals(value))
                    IoC.Get<IShell>().ActiveItem.UndoRedoManager.ExecuteAction(
                        new ChangeObjectValueAction(BoundPropertyDescriptor, _originalValue, value));
            }
            finally
            {
                _originalValue = null;
                _undoEnabled = true;
            }
        }

        public override void Dispose()
        {
            OnEndEdit();
            base.Dispose();
        }
    }
}
