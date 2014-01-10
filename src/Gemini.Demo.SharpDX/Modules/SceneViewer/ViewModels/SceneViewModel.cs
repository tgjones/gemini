using System.ComponentModel.Composition;
using Gemini.Framework;
using SharpDX;

namespace Gemini.Demo.SharpDX.Modules.SceneViewer.ViewModels
{
    [Export(typeof(SceneViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
	public class SceneViewModel : Document
	{
	    public override bool ShouldReopenOnStart
	    {
		    get { return true; }
	    }

	    private Vector3 _position;

	    public Vector3 Position
	    {
            get { return _position; }
            set
            {
                _position = value;
                NotifyOfPropertyChange(() => Position);
            }
	    }

        public SceneViewModel()
        {
            DisplayName = "3D Scene";
        }
	}
}