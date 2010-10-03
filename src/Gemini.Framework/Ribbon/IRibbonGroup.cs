using System.Collections.Generic;

namespace Gemini.Framework.Ribbon
{
	public interface IRibbonGroup
	{
		string Name { get; }
		string Title { get; set; }
		IEnumerable<IRibbonItem> Items { get; }

		void Add(IRibbonItem item);
	}
}