using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemini.Demo.Modules.WebPage.ViewModels
{
    [DisplayName("Web Browser")]
    [Export(typeof(WebPageViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class WebPageViewModel : WebPageDocument
    {
        public override bool ShouldReopenOnStart
        {
            get
            {
                return true;
            }
        }

        public WebPageViewModel()
        {
            DisplayName = "Web Browser";
        }

        public override void SaveState(BinaryWriter writer)
        {
            if (LoadedUri == null)
                writer.Write(TargetUri);
            else
                writer.Write(LoadedUri.ToString());
            writer.Write(TitleOfPage);
        }

        public override void LoadState(BinaryReader reader)
        {
            TargetUri = reader.ReadString();
            TitleOfPage = reader.ReadString();

            DisplayName = TitleOfPage;
        }
    }
}
