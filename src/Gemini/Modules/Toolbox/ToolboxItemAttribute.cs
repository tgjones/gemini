using System;

namespace Gemini.Modules.Toolbox
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ToolboxItemAttribute : Attribute
    {
        public Type DocumentType { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string IconSource { get; set; }

        public ToolboxItemAttribute(Type documentType, string name, string category, string iconSource = null)
        {
            DocumentType = documentType;
            Name = name;
            Category = category;
            IconSource = iconSource;
        }
    }
}