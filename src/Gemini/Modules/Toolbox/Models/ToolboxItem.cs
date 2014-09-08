using System;

namespace Gemini.Modules.Toolbox.Models
{
    public class ToolboxItem
    {
        public Type DocumentType { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public Uri IconSource { get; set; }
        public Type ItemType { get; set; }
    }
}