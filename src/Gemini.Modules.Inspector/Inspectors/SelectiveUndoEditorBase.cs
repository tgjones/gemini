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
    /// Begin/end notifications are idempotent within an edit group, and
    /// <see langword="null"/> is a valid captured value.
    /// </summary>
    /// <typeparam name="TValue">Type of the value</typeparam>
    public abstract class SelectiveUndoEditorBase<TValue> : EditorBase<TValue>, IDisposable
    {
        private object _originalValue;
        private bool _isEditing;

        protected void OnBeginEdit()
        {
            if (_isEditing)
                return;

            IsUndoEnabled = false;
            _originalValue = RawValue;
            _isEditing = true;
        }

        protected void OnEndEdit()
        {
            if (!_isEditing)
                return;

            try
            {
                var value = RawValue;
                if (!Equals(_originalValue, value))
                    IoC.Get<IShell>().ActiveItem.UndoRedoManager.ExecuteAction(
                        new ChangeObjectValueAction(BoundPropertyDescriptor, _originalValue, value, StringConverter));
            }
            finally
            {
                _originalValue = null;
                _isEditing = false;
                IsUndoEnabled = true;
            }
        }

        public override void Dispose()
        {
            OnEndEdit();
            base.Dispose();
        }
    }
}
