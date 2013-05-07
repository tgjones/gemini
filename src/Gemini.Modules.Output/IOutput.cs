using System.IO;
using Gemini.Framework;

namespace Gemini.Modules.Output
{
	public interface IOutput : ITool
	{
		TextWriter Writer { get; }
		void AppendLine(string text);
		void Append(string text);
		void Clear();
	}
}