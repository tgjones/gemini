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

        public MenuItem(string name, string text)
            : this(name, text, null, null) { }

		public MenuItem(string text, Func<IEnumerable<IResult>> execute)
			: base(text)
		{
			_execute = execute;
		}

        public MenuItem(string name, string text, Func<IEnumerable<IResult>> execute) 
            : this(name, text, execute, null) { }

		public MenuItem(string text, Func<IEnumerable<IResult>> execute, Func<bool> canExecute)
			: base(text, canExecute)
		{
			_execute = execute;
		}

        public MenuItem(string name, string text, Func<IEnumerable<IResult>> execute, Func<bool> canExecute)
            : base(name, text, canExecute)
        {
            this._execute = execute;
        }

		#endregion

		public IEnumerable<IResult> Execute()
		{
			return _execute != null && CanExecute ? _execute() : new IResult[] { };
		}
	}
}
