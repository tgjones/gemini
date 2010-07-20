using System;
using System.ComponentModel.Composition;

namespace Gemini.Pads.Toolbox
{
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ProvideToolboxControlAttribute : ExportAttribute
	{
		public string Category { get; set; }
		public string Name { get; set; }

		public ProvideToolboxControlAttribute(string category, string name)
			: base("Gemini.Pads.Toolbox.ProvidesToolboxControlAttribute")
		{
			Category = category;
			Name = name;
		}
	}
}