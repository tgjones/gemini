using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Gemini.Modules.Inspector.Inspectors;

namespace Gemini.Modules.Inspector.Conventions
{
    public class RangePropertyEditorBuilder : PropertyEditorBuilder
    {
        public override bool IsApplicable(PropertyDescriptor propertyDescriptor)
        {
            var isNumberType = propertyDescriptor.PropertyType == typeof(int)
                || propertyDescriptor.PropertyType == typeof(double)
                || propertyDescriptor.PropertyType == typeof(float);

            if (!isNumberType)
                return false;

            return propertyDescriptor.Attributes.Cast<Attribute>().Any(x => x is RangeAttribute);
        }

        public override IEditor BuildEditor(PropertyDescriptor propertyDescriptor)
        {
            var rangeAttribute = propertyDescriptor.Attributes
                .Cast<Attribute>().OfType<RangeAttribute>()
                .First();

            if (propertyDescriptor.PropertyType == typeof(int))
               return new RangeEditorViewModel<int>((int) rangeAttribute.Minimum, (int) rangeAttribute.Maximum);

            if (propertyDescriptor.PropertyType == typeof(double))
               return new RangeEditorViewModel<double>((double) rangeAttribute.Minimum, (double) rangeAttribute.Maximum);

            if (propertyDescriptor.PropertyType == typeof(float))
               return new RangeEditorViewModel<float>((float) rangeAttribute.Minimum, (float) rangeAttribute.Maximum);

            throw new InvalidOperationException();
        }
    }
}