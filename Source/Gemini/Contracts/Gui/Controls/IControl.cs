using System.Windows;
using Gemini.Contracts.Conditions;
using Gemini.Contracts.Services.ExtensionService;

namespace Gemini.Contracts.Gui.Controls
{
	public interface IControl : IExtension
	{
		string ToolTip { get; }
		bool Visible { get; }
		ICondition VisibleCondition { get; set; }
		Thickness Padding { get; }
		Thickness Margin { get; }
	}
}