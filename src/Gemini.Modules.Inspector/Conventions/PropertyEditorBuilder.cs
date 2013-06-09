using System.ComponentModel;
using Gemini.Modules.Inspector.Inspectors;

namespace Gemini.Modules.Inspector.Conventions
{
    public abstract class PropertyEditorBuilder
    {
        public abstract bool IsApplicable(PropertyDescriptor propertyDescriptor);
        public abstract IEditor BuildEditor(PropertyDescriptor propertyDescriptor);
    }
}