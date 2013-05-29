using System.Collections.Generic;

namespace Gemini.Modules.CodeCompiler
{
    public interface ICodeCompiler
    {
        void Compile(IEnumerable<string> fileNames, IEnumerable<string> references, string outputName);
    }
}