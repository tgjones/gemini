using Gemini.Framework;

namespace Gemini.Modules.Inspector
{
	public interface IInspectorTool : ITool
	{
		object SelectedObject { get; set; }
	}
}