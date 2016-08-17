using Gemini.Demo.Modules.WebPage.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemini.Demo.Modules.WebPage
{
    [Export(typeof(IEditorProvider))]
    public class EditorProvider : IEditorProvider
    {
        private readonly List<string> _extensions = new List<string>
        {
            ".html", ".htm",
            ".pdf",
            ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".svg", //image files
            ".wav", ".mid", ".midi", ".wma", ".mp3", ".ogg", ".rma", //music files
            ".avi", ".mp4", ".divx", ".wmv", //movie files
        };

        public IEnumerable<EditorFileType> FileTypes
        {
            get
            {
                yield return new EditorFileType("HTML File", ".html");
                yield return new EditorFileType("HTML File", ".htm");

                yield return new EditorFileType("PDF File", ".pdf");

                yield return new EditorFileType("Media File", ".png");
                yield return new EditorFileType("Media File", ".jpg");
                yield return new EditorFileType("Media File", ".jpeg");
                yield return new EditorFileType("Media File", ".gif");
                yield return new EditorFileType("Media File", ".svg");

                yield return new EditorFileType("Media File", ".wav");
                yield return new EditorFileType("Media File", ".mid");
                yield return new EditorFileType("Media File", ".midi");
                yield return new EditorFileType("Media File", ".wma");
                yield return new EditorFileType("Media File", ".mp3");
                yield return new EditorFileType("Media File", ".ogg");
                yield return new EditorFileType("Media File", ".rma");
                yield return new EditorFileType("Media File", ".avi");
                yield return new EditorFileType("Media File", ".mp4");
                yield return new EditorFileType("Media File", ".divx");
                yield return new EditorFileType("Media File", ".wmv");
            }
        }

        public bool CanCreateNew
        {
            get { return false; }
        }

        public bool Handles(string path)
        {
            var extension = Path.GetExtension(path);
            return _extensions.Contains(extension);
        }

        public IDocument Create()
        {
            return new WebPageViewModel();
        }

        public Task New(IDocument document, string name)
        {
            //await ((EditorViewModel)document).New(name);

            return TaskUtility.Completed;
        }

        public async Task Open(IDocument document, string path)
        {
            await ((WebPageViewModel)document).Load(path);
        }
    }
}
