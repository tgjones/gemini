namespace Gemini.Framework.Services
{
    public abstract class EditorItemType
    {
        public string Name { get; }

        protected EditorItemType(string name)
        {
            Name = name;
        }
    }

    public class EditorFileType : EditorItemType
    {
        public string FileExtension { get; }

        public EditorFileType(string name, string fileExtension) : base(name)
        {
            FileExtension = fileExtension;
        }
    }
    public class EditorFolderType : EditorItemType
    {
        public EditorFolderType(string name) : base(name)
        {

        }
    }
}