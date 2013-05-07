using Gemini.Framework;

namespace Gemini.Modules.PropertyGrid
{
	public interface IPropertyGrid : ITool
	{
		object SelectedObject { get; set; }
	}
}