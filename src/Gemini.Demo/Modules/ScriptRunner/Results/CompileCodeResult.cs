using System;
using Caliburn.Micro;
using Gemini.Modules.CodeCompiler;

namespace Gemini.Demo.Modules.ScriptRunner.Results
{
    public class CompileCodeResult : IResult
    {
        public event EventHandler<ResultCompletionEventArgs> Completed;

        private readonly ICodeCompiler _codeCompiler;
        private readonly string _path;

        public CompileCodeResult(ICodeCompiler codeCompiler, string path)
        {
            _codeCompiler = codeCompiler;
            _path = path;
        }

        public void Execute(ActionExecutionContext context)
        {
            _codeCompiler.Compile(
                new[] { _path },
                new[] { "mscorlib" },
                "GeminiDemoScript");

            if (Completed != null)
                Completed(this, new ResultCompletionEventArgs());
        }
    }
}