using Gemini.Framework;

namespace Gemini.Modules.Inspector
{
	public interface IInspectorTool : ITool
	{
        IInspectableObject SelectedObject { get; set; }
	}
}