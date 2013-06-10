using System;
using System.ComponentModel;

namespace Gemini.Modules.Inspector
{
    public class BoundPropertyDescriptor
    {
        public event EventHandler ValueChanged
        {
            add { PropertyDescriptor.AddValueChanged(PropertyOwner, value); }
            remove { PropertyDescriptor.RemoveValueChanged(PropertyOwner, value); }
        }

        public static BoundPropertyDescriptor FromProperty(object propertyOwner, string propertyName)
        {
            // TODO: Cache all this.
            var properties = TypeDescriptor.GetProperties(propertyOwner);
            return new BoundPropertyDescriptor(propertyOwner, properties.Find(propertyName, false));
        }

        public PropertyDescriptor PropertyDescriptor { get; private set; }
        public object PropertyOwner { get; private set; }

        public object Value
        {
            get { return PropertyDescriptor.GetValue(PropertyOwner); }
            set { PropertyDescriptor.SetValue(PropertyOwner, value); }
        }

        public BoundPropertyDescriptor(object propertyOwner, PropertyDescriptor propertyDescriptor)
        {
            PropertyOwner = propertyOwner;
            PropertyDescriptor = propertyDescriptor;
        }
    }
}