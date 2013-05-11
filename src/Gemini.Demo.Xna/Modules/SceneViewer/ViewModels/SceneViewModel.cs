using System;
using Gemini.Framework;
using Microsoft.Xna.Framework;

namespace Gemini.Demo.Xna.Modules.SceneViewer.ViewModels
{
	public class SceneViewModel : Document
	{
		public override string DisplayName
		{
			get { return "3D Scene"; }
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