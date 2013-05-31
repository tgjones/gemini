using Caliburn.Micro;
using Gemini.Modules.UndoRedo;

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
                IoC.Get<IUndoRedoManager>().ExecuteAction(
                    new ChangeObjectValueAction(BoundPropertyDescriptor, value));
                NotifyOfPropertyChange(() => Value);
            }
        }
    }
}