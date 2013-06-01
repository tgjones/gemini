using Gemini.Modules.UndoRedo;

namespace Gemini.Modules.Inspector.Inspectors
{
    public class ChangeObjectValueAction : IUndoableAction
    {
        private readonly BoundPropertyDescriptor _boundPropertyDescriptor;
        private readonly object _originalValue;
        private readonly object _newValue;

        public string Name
        {
            get
            {
                return string.Format("Change {0} from {1} to {2}",
                    _boundPropertyDescriptor.PropertyDescriptor.DisplayName,
                    _originalValue,
                    _newValue);
            }
        }

        public ChangeObjectValueAction(BoundPropertyDescriptor boundPropertyDescriptor, object newValue)
        {
            _boundPropertyDescriptor = boundPropertyDescriptor;
            _originalValue = boundPropertyDescriptor.Value;
            _newValue = newValue;
        }

        public void Execute()
        {
            _boundPropertyDescriptor.Value = _newValue;
        }

        public void Undo()
        {
            _boundPropertyDescriptor.Value = _originalValue;
        }
    }
}