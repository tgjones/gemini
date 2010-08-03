using System;
using System.Collections.Generic;
using Caliburn.PresentationFramework;

namespace Gemini.Framework.Ribbon
{
	public class RibbonButton : RibbonButtonBase<RibbonButton>
	{
		private readonly Func<IEnumerable<IResult>> _execute;

		public RibbonButton(string text)
			: base(text)
		{
			
		}

		public RibbonButton(string text, Func<IEnumerable<IResult>> execute)
			: base(text)
		{
			_execute = execute;
		}

		public RibbonButton(string text, Func<IEnumerable<IResult>> execute, Func<bool> canExecute)
			: base(text, canExecute)
		{
			_execute = execute;
		}

		public IEnumerable<IResult> Execute()
		{
			return _execute != null ? _execute() : new IResult[] { };
		}
	}
}