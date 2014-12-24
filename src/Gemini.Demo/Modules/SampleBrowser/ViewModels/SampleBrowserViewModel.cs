using System.ComponentModel.Composition;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Gemini.Demo.Modules.SampleBrowser.ViewModels
{
    [Export(typeof(SampleBrowserViewModel))]
    public class SampleBrowserViewModel : Document
    {
        private readonly IShell _shell;
        private readonly ISample[] _samples;

        public override string DisplayName
        {
            get { return "Sample Browser"; }
        }

        public ISample[] Samples
        {
            get { return _samples; }
        }

        [ImportingConstructor]
        public SampleBrowserViewModel([Import] IShell shell,
            [ImportMany] ISample[] samples)
        {
            _shell = shell;
            _samples = samples;
        }

        public void Activate(ISample sample)
        {
            sample.Activate(_shell);
        }
    }
}
