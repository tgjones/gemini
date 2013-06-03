using System;
using System.Collections.Generic;
using Caliburn.Micro;

namespace Gemini.Modules.ToolBars.Models
{
	public class ToolBarItem : StandardToolBarItem
	{
		private readonly Func<IEnumerable<IResult>> _execute;

		#region Constructors

		public ToolBarItem(string text)
			: base(text)
		{
			
		}

		public ToolBarItem(string text, Func<IEnumerable<IResult>> execute)
			: base(text)
		{
			_execute = execute;
		}

        public ToolBarItem(string text, Func<IEnumerable<IResult>> execute, Func<bool> canExecute)
			: base(text, canExecute)
		{
			_execute = execute;
		}

		#endregion

		public IEnumerable<IResult> Execute()
		{
			return _execute != null ? _execute() : new IResult[] { };
		}
	}
}