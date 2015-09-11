using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Reflection;

namespace Gemini.Modules.CodeCompiler
{
    public interface ICodeCompiler
    {
        Assembly Compile(
            IEnumerable<SyntaxTree> syntaxTrees,
            IEnumerable<MetadataReference> references,
            string outputName);
    }
}