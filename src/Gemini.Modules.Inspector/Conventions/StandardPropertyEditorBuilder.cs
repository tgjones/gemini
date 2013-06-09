using System.ComponentModel;
using Gemini.Modules.Inspector.Inspectors;

namespace Gemini.Modules.Inspector.Conventions
{
    public class StandardPropertyEditorBuilder<T, TEditor> : PropertyEditorBuilder
        where TEditor : IEditor, new()
    {
        public override bool IsApplicable(PropertyDescriptor propertyDescriptor)
        {
            return propertyDescriptor.PropertyType == typeof(T);
        }

        public override IEditor BuildEditor(PropertyDescriptor propertyDescriptor)
        {
            return new TEditor();
        }
    }
}