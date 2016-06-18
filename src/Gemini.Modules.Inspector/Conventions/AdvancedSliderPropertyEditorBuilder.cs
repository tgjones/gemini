using System;
using System.ComponentModel;
using Gemini.Modules.Inspector.Inspectors;

namespace Gemini.Modules.Inspector.Conventions
{
    public class AdvancedSliderPropertyEditorBuilder : PropertyEditorBuilder
    {
        public override bool IsApplicable(PropertyDescriptor propertyDescriptor)
        {
            var isNumberType = propertyDescriptor.PropertyType == typeof(int)
                || propertyDescriptor.PropertyType == typeof(double)
                || propertyDescriptor.PropertyType == typeof(float);

            return isNumberType;
        }

        public override IEditor BuildEditor(PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.PropertyType == typeof(int))
            {
                return new AdvancedSliderEditorViewModel<int>() {
                    Speed = 1
                };
            }

            if (propertyDescriptor.PropertyType == typeof(double))
            {
                return new AdvancedSliderEditorViewModel<double>() {
                    Speed = 0.1
                };
            }

            if (propertyDescriptor.PropertyType == typeof(float))
            {
                return new AdvancedSliderEditorViewModel<float>() {
                    Speed = 0.1f
                };
            }

            throw new InvalidOperationException();
        }
    }
}
