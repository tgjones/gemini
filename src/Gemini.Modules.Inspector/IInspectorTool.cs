using System;
using Gemini.Framework;

namespace Gemini.Modules.Inspector
{
	public interface IInspectorTool : ITool
	{
	    event EventHandler SelectedObjectChanged;
        IInspectableObject SelectedObject { get; set; }
	}
}