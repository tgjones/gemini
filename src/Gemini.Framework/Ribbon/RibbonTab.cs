using System.Collections.Generic;
using System.Linq;

namespace Gemini.Framework.Ribbon
{
	public class RibbonTab : IRibbonTab
	{
		private List<IRibbonGroup> _groups;

		public string Title { get; set; }

		public IEnumerable<IRibbonGroup> Groups
		{
			get { return _groups; }
			set { _groups = value.ToList(); }
		}

		public string Name
		{
			get { return string.IsNullOrEmpty(Title) ? null : Title.Replace("_", string.Empty); }
		}

		public RibbonTab(string title, IEnumerable<IRibbonGroup> groups)
			: this(title)
		{
			Groups = groups;
		}

		public RibbonTab(string title)
		{
			_groups = new List<IRibbonGroup>();
			Title = title;
		}

		public void Add(IRibbonGroup group)
		{
			_groups.Add(group);
		}
	}
}