using System;
using System.Collections.Generic;
using Caliburn.PresentationFramework;

namespace Gemini.Framework.Ribbon
{
	public class RibbonCheckBox : RibbonToggleButton
	{
		public RibbonCheckBox(string text, Func<bool, IEnumerable<IResult>> execute = null, Func<bool> canExecute = null, string groupName = null, Sizes sizes = Sizes.Middle)
			: base(text, execute, canExecute, groupName, sizes)
		{
			
		}
	}
}