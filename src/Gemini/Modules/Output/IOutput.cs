using Gemini.Framework;

namespace Gemini.Modules.Output
{
	public interface IOutput : ITool
	{
		void Append(string text);
		void Clear();
	}
}