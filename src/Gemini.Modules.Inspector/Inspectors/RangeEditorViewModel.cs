namespace Gemini.Modules.Inspector.Inspectors
{
    public class RangeEditorViewModel<T> : SelectiveUndoEditorBase<T>, ILabelledInspector
    {
        private readonly T _minimum;
        private readonly T _maximum;

        public T Minimum
        {
            get { return _minimum; }
        }

        public T Maximum
        {
            get { return _maximum; }
        }

        public RangeEditorViewModel(T minimum, T maximum)
        {
            _minimum = minimum;
            _maximum = maximum;
        }

        public void DragStarted()
        {
            OnBeginEdit();
        }

        public void DragCompleted()
        {
            OnEndEdit();
        }
    }
}