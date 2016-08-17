using Gemini.Framework;
using Gemini.Framework.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemini.Demo.Modules.WebPage
{
    public class WebPageDocument : Document, IWebPageDocument
    {
        string _targetUri;
        public string TargetUri
        {
            get { return _targetUri; }
            set
            {
                if (value != null)
                {
                    _targetUri = value;
                    NotifyOfPropertyChange(() => TargetUri);
                }
            }
        }

        Uri _loadedUri;
        public Uri LoadedUri
        {
            get { return _loadedUri; }
            set
            {
                _loadedUri = value;
                NotifyOfPropertyChange(() => LoadedUri);
            }
        }

        string _titleOfPage;
        public string TitleOfPage
        {
            get { return _titleOfPage; }
            set
            {
                _titleOfPage = value;
                NotifyOfPropertyChange(() => TitleOfPage);
            }
        }

        bool _isPageLoading;
        public bool IsPageLoading
        {
            get { return _isPageLoading; }
            set
            {
                _isPageLoading = value;
                NotifyOfPropertyChange(() => IsPageLoading);
                UpdateDisplayName();
            }
        }

        private void UpdateDisplayName()
        {
            DisplayName = (IsPageLoading) ? "Loading..." : ((TitleOfPage == null) ? "Web Browser" : TitleOfPage);
        }

        public Task Load(string filePath)
        {
            TargetUri = new Uri(filePath).AbsoluteUri;
            return TaskUtility.Completed;
        }
    }
}
