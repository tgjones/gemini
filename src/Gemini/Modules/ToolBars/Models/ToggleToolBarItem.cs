using System;
using System.Collections.Generic;
using Caliburn.Micro;

namespace Gemini.Modules.ToolBars.Models
{
	public class ToggleToolBarItem : StandardToolBarItem
	{
		private readonly Func<bool, IEnumerable<IResult>> _execute;

		private bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set { _isChecked = value; NotifyOfPropertyChange(() => IsChecked); }
		}

		#region Constructors

		public ToggleToolBarItem(string text)
			: base(text)
		{
			
		}

		public ToggleToolBarItem(string text, Func<bool, IEnumerable<IResult>> execute)
			: base(text)
		{
			_execute = execute;
		}

        public ToggleToolBarItem(string text, Func<bool, IEnumerable<IResult>> execute, Func<bool> canExecute)
			: base(text, canExecute)
		{
			_execute = execute;
		}

		#endregion

		public IEnumerable<IResult> Execute()
		{
			return _execute != null ? _execute(IsChecked) : new IResult[] { };
		}
	}
}