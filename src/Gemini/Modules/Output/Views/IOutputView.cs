namespace Gemini.Modules.Output.Views
{
	public interface IOutputView
	{
		void ScrollToEnd();
		void SetText(string text);
	}
}