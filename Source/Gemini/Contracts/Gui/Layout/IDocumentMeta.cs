using System;
using System.ComponentModel.Composition;

namespace Gemini.Contracts.Gui.Layout
{
	public interface IDocumentMeta : ILayoutItemMeta
	{
	}

	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class DocumentAttribute : LayoutItemAttribute
	{
		public DocumentAttribute() : base(typeof(IDocument)) { }
	}
}