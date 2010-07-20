using System;
using System.ComponentModel.Composition;

namespace Gemini.Contracts.Gui.Layout
{
	public interface IPadMeta : ILayoutItemMeta
	{
	}

	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class PadAttribute : LayoutItemAttribute
	{
		public PadAttribute() : base(typeof(IPad)) { }
	}
}