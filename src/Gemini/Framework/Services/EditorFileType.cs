using System;

namespace Gemini.Framework.Services
{
    public class EditorFileType
    {
        public string Name { get; set; }
        public string FileExtension { get; set; }
        public Uri IconSource { get; set; }

        public EditorFileType(string name, string fileExtension, Uri iconSource = null)
        {
            Name = name;
            FileExtension = fileExtension;
            IconSource = iconSource;
        }

        public EditorFileType()
        {
            
        }
    }
}