using Gemini.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemini.Demo.Modules.WebPage
{
    public interface IWebPageDocument : IDocument
    {
        string TargetUri { get; }
        string TitleOfPage { get; }
        bool IsPageLoading { get; }

        Task Load(string targetUri);
    }
}
