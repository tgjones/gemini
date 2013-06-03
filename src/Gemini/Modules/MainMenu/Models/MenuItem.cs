using System;
using System.Collections.Generic;
using Caliburn.Micro;

namespace Gemini.Modules.MainMenu.Models
{
	public class MenuItem : StandardMenuItem
	{
		private readonly Func<IEnumerable<IResult>> _execute;

		#region Constructors

		public MenuItem(string text)
			: base(text)
		{
			
		}

		public MenuItem(string text, Func<IEnumerable<IResult>> execute)
			: base(text)
		{
			_execute = execute;
		}

		public MenuItem(string text, Func<IEnumerable<IResult>> execute, Func<bool> canExecute)
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