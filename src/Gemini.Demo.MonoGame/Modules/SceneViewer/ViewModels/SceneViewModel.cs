using System;
using System.ComponentModel.Composition;
using Gemini.Demo.MonoGame.Modules.SceneViewer.Views;
using Gemini.Framework;
using Microsoft.Xna.Framework;

namespace Gemini.Demo.MonoGame.Modules.SceneViewer.ViewModels
{
    [Export(typeof(SceneViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
	public class SceneViewModel : Document
    {
        private ISceneView _sceneView;

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

                if (_sceneView != null)
                    _sceneView.Invalidate();
            }
	    }

        public SceneViewModel()
        {
            DisplayName = "3D Scene";
        }

        protected override void OnViewLoaded(object view)
        {
            _sceneView = view as ISceneView;
            base.OnViewLoaded(view);
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                var view = GetView() as IDisposable;
                if (view != null)
                    view.Dispose();
            }

            base.OnDeactivate(close);
        }
	}
}