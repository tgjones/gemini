using Caliburn.Micro;

namespace Gemini.Modules.Output
{
	public interface IOutput : IScreen
	{
		void Append(string text);
		void Clear();
	}
}