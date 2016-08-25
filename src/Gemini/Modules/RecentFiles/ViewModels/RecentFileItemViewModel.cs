using Caliburn.Micro;

namespace Gemini.Modules.RecentFiles.ViewModels
{
    public class RecentFileItemViewModel : PropertyChangedBase
    {
        private int _index;
        public int Index
        {
            get { return _index; }
            internal set
            {
                _index = value;
                NotifyOfPropertyChange(() => Index);
            }
        }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                _displayName = ShortenPath(_filePath);
                NotifyOfPropertyChange(() => FilePath);
            }
        }

        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
        }

        // TODO: will implement Pinned
        private bool _pinned = false;
        public bool Pinned
        {
            get { return _pinned; }
            set
            {
                _pinned = value;
                NotifyOfPropertyChange(() => Pinned);
            }
        }

        public RecentFileItemViewModel(string filePath, bool pinned = false)
        {
            _filePath = filePath;
            _displayName = ShortenPath(filePath);

            _pinned = pinned;
        }

        // http://stackoverflow.com/questions/8360360/function-to-shrink-file-path-to-be-more-human-readable
        private string ShortenPath(string path, int maxLength = 50)
        {
            string[] splits = path.Split('\\');

            string output = "";

            if (splits.Length > 4)
                output = splits[0] + "\\" + splits[1] + "\\...\\" + splits[splits.Length - 2] + "\\" + splits[splits.Length - 1];
            else
                output = string.Join("\\", splits, 0, splits.Length);

            return output;
        }
    }
}