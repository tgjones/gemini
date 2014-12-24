using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Demo.Modules.SampleBrowser;
using Gemini.Demo.Modules.SharpDXSceneViewer.ViewModels;
using Gemini.Framework.Services;

namespace Gemini.Demo.Modules.SharpDXSceneViewer
{
    [Export(typeof(ISample))]
    public class Sample : ISample
    {
        public string Name
        {
            get { return "SharpDX"; }
        }

        public void Activate(IShell shell)
        {
            shell.OpenDocument(IoC.Get<SceneViewModel>());
        }
    }
}