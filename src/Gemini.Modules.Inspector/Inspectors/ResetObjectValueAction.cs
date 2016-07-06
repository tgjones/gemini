using Gemini.Modules.Inspector.Properties;
using Gemini.Modules.UndoRedo;
using System.Globalization;
using System.Windows.Data;

namespace Gemini.Modules.Inspector.Inspectors
{
    public class ResetObjectValueAction : IUndoableAction
    {
        private readonly BoundPropertyDescriptor _boundPropertyDescriptor;
        private readonly object _originalValue;
        private object _newValue;
        private readonly IValueConverter _stringConverter;

        public string Name
        {
            get
            {
                string origText;
                string newText;

                if (_stringConverter != null)
                {
                    origText = (string) _stringConverter.Convert(_originalValue, typeof(string), null, CultureInfo.CurrentUICulture);
                    newText = (string) _stringConverter.Convert(_newValue, typeof(string), null, CultureInfo.CurrentUICulture);
                }
                else
                {
                    origText = _originalValue.ToString();
                    newText = _newValue.ToString();
                }

                return string.Format(Resources.ResetObjectValueActionFormat,
                    _boundPropertyDescriptor.PropertyDescriptor.DisplayName,
                    origText,
                    newText);
            }
        }

        public ResetObjectValueAction(BoundPropertyDescriptor boundPropertyDescriptor, IValueConverter stringConverter) :
            this(boundPropertyDescriptor, boundPropertyDescriptor.Value, stringConverter)
        { }

        public ResetObjectValueAction(BoundPropertyDescriptor boundPropertyDescriptor, object originalValue, IValueConverter stringConverter)
        {
            _boundPropertyDescriptor = boundPropertyDescriptor;
            _originalValue = originalValue;
            _stringConverter = stringConverter;
        }

        public void Execute()
        {
            _boundPropertyDescriptor.PropertyDescriptor.ResetValue(_boundPropertyDescriptor.PropertyOwner);
            _newValue = _boundPropertyDescriptor.Value;
        }

        public void Undo()
        {
            _boundPropertyDescriptor.Value = _originalValue;
        }
    }
}
