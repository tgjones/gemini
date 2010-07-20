namespace Gemini.Contracts.Gui.Controls
{
	public interface IProgressBar : IControl
	{
		double Minimum { get; }
		double Maximum { get; }
		double Value { get; }
	}
}