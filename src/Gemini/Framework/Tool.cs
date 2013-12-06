using System.Windows.Input;
using Gemini.Framework.Services;

namespace Gemini.Framework
{
	public abstract class Tool : LayoutItemBase, ITool
	{
		private ICommand _closeCommand;
		public override ICommand CloseCommand
		{
			get { return _closeCommand ?? (_closeCommand = new RelayCommand(p => IsVisible = false, p => true)); }
		}

	    public abstract PaneLocation PreferredLocation { get; }

	    public virtual double PreferredWidth
	    {
            get { return 200; }
	    }

        public virtual double PreferredHeight
        {
            get { return 200; }
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

        public override bool ShouldReopenOnStart
        {
            // Tool windows should always reopen on app start by default.
            get { return true; }
        }

		protected Tool()
		{
			IsVisible = true;
		}
	}
}