using System.Collections.Generic;

namespace Gemini.Framework.Ribbon
{
	public interface IRibbonTab
	{
		string Name { get; }
		string Title { get; set; }
		IEnumerable<IRibbonGroup> Groups { get; }

		void Add(IRibbonGroup group);
	}
}