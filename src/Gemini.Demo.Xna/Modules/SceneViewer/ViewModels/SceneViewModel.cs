using System;
using Gemini.Framework;

namespace Gemini.Demo.Xna.Modules.SceneViewer.ViewModels
{
	public class SceneViewModel : Document
	{
		public override string DisplayName
		{
			get { return "3D Scene"; }
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