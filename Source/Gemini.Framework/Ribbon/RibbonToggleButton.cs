using System;
using System.Collections.Generic;
using Caliburn.PresentationFramework;

namespace Gemini.Framework.Ribbon
{
	public class RibbonToggleButton : RibbonButtonBase<RibbonToggleButton>
	{
		private readonly Func<bool, IEnumerable<IResult>> _execute;
		private bool _isChecked;
		private readonly string _groupName;

		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				NotifyOfPropertyChange("IsChecked");
				if (_execute != null)
					new SequentialResult(_execute.Invoke(_isChecked)).Execute(null, null);
			}
		}

		public string GroupName
		{
			get { return _groupName; }
		}

		public RibbonToggleButton(string text, Func<bool, IEnumerable<IResult>> execute = null, Func<bool> canExecute = null, string groupName = null, Sizes sizes = Sizes.Middle)
			: base(text, canExecute, sizes)
		{
			_execute = execute;
			_groupName = groupName;
		}
	}
}