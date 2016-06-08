namespace Gemini.Modules.Inspector.Inspectors
{
    /// <summary>
    /// This class is used as an editor base for values that do not support
    /// undo / redo operations, for example write only variables.
    /// </summary>
    /// <typeparam name="TValue">Type of the value</typeparam>
    public abstract class NoUndoEditorBase<TValue> : EditorBase<TValue>
    {
        public override TValue Value
        {
            get { return (TValue) BoundPropertyDescriptor.Value; }
            set { NotifyOfPropertyChange(() => Value); }
        }
    }
}
