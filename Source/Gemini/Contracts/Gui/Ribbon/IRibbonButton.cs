namespace Gemini.Contracts.Gui.Ribbon
{
	public interface IRibbonButton : IRibbonItem
	{
		string Text { get; }
		string SizeDefinition { get; }
	}
}