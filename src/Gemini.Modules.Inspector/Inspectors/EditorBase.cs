namespace Gemini.Modules.Inspector.Inspectors
{
    public abstract class EditorBase<TValue> : InspectorBase, IEditor
    {
        public override string Name
        {
            get { return BoundPropertyDescriptor.PropertyDescriptor.DisplayName; }
        }

        public BoundPropertyDescriptor BoundPropertyDescriptor { get; set; }

        public TValue Value
        {
            get { return (TValue) BoundPropertyDescriptor.Value; }
            set
            {
                BoundPropertyDescriptor.Value = value;
                NotifyOfPropertyChange(() => Value);
            }
        }
    }
}