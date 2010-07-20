namespace Gemini.Contracts.Services.OutputService
{
	public interface IOutputService
	{
		void Append(string text);
		void Clear();
	}
}