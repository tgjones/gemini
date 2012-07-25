using System;
using Gemini.Framework.Services;

namespace Gemini.Framework
{
	public abstract class Tool : LayoutItemBase, ITool
	{
		public abstract PaneLocation PreferredLocation { get; }

		public virtual Uri IconSource
		{
			get { return null; }
		}

		private bool _isVisible;
		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				_isVisible = value;
				NotifyOfPropertyChange(() => IsVisible);
			}
		}

		protected Tool()
		{
			IsVisible = true;
		}
	}
}