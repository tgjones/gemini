using System.Collections.Generic;
using System.Reflection;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;

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