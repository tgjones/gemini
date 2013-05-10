using System.ComponentModel;

namespace Gemini.Modules.Inspector
{
    public class BoundPropertyDescriptor
    {
        public static BoundPropertyDescriptor FromProperty(object propertyOwner, string propertyName)
        {
            // TODO: Cache all this.
            var properties = TypeDescriptor.GetProperties(propertyOwner);
            return new BoundPropertyDescriptor
            {
                PropertyDescriptor = properties.Find(propertyName, false),
                PropertyOwner = propertyOwner
            };
        }

        public PropertyDescriptor PropertyDescriptor { get; set; }
        public object PropertyOwner { get; set; }

        public object Value
        {
            get { return PropertyDescriptor.GetValue(PropertyOwner); }
            set { PropertyDescriptor.SetValue(PropertyOwner, value); }
        }
    }
}