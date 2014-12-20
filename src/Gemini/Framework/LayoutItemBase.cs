using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using Caliburn.Micro;

namespace Gemini.Framework
{
	public abstract class LayoutItemBase : Screen, ILayoutItem
	{
		private readonly Guid _id = Guid.NewGuid();
		
		public abstract ICommand CloseCommand { get; }

        [Browsable(false)]
		public Guid Id
		{
			get { return _id; }
		}

        [Browsable(false)]
		public string ContentId
		{
			get { return _id.ToString(); }
		}

        [Browsable(false)]
		public virtual Uri IconSource
		{
			get { return null; }
		}

		private bool _isSelected;

        [Browsable(false)]
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				NotifyOfPropertyChange(() => IsSelected);
			}
		}

        [Browsable(false)]
        public virtual bool ShouldReopenOnStart
        {
            get { return false; }
        }

		public virtual void LoadState(BinaryReader reader)
		{
		}

		public virtual void SaveState(BinaryWriter writer)
		{
		}
	}
}