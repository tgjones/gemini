using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Reflection.Emit;
using Caliburn.Micro;
using Gemini.Framework.Results;
using Gemini.Framework.Services;
using Gemini.Modules.ErrorList;
using Gemini.Modules.Output;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using Microsoft.CodeAnalysis.Emit;

namespace Gemini.Modules.CodeCompiler
{
    [Export(typeof(ICodeCompiler))]
    public class CodeCompiler : ICodeCompiler
    {
        private readonly IEditorProvider _editorProvider;
        private readonly IOutput _output;
        private readonly IErrorList _errorList;

        [ImportingConstructor]
        public CodeCompiler(IEditorProvider editorProvider, IOutput output, IErrorList errorList)
        {
            _editorProvider = editorProvider;
            _output = output;
            _errorList = errorList;
        }

        public Assembly Compile(IEnumerable<SyntaxTree> syntaxTrees, IEnumerable<MetadataReference> references, string outputName)
        {
            _output.AppendLine("------ Compile started");

            GC.Collect();
            var compilation = CSharpCompilation.Create(outputName)
                       .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                       .AddReferences(references)
                       .AddSyntaxTrees(syntaxTrees);

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                _errorList.Items.Clear();
                if (!result.Success)
                {
                    ProcessResult(result);
                    _output.AppendLine("------ Compile failed");
                    return null;
                }
                _output.AppendLine("------ Compile finished");
                ms.Seek(0, SeekOrigin.Begin);
                return Assembly.Load(ms.ToArray());
            }
        }

        private void ProcessResult(EmitResult result)
        {
            foreach (var diagnostic in result.Diagnostics)
            {
                if (!diagnostic.Location.IsInSource)
                {
                    throw new NotSupportedException("Only support source file locations.");
                }
                var itemType = GetItemType(diagnostic.Severity);
                var description = diagnostic.GetMessage();
                var lineSpan = diagnostic.Location.GetLineSpan();
                _errorList.AddItem(itemType, description,
                    lineSpan.Path,
                    lineSpan.StartLinePosition.Line,
                    lineSpan.StartLinePosition.Character,
                    () =>
                    {
                        var openDocumentResult = new OpenDocumentResult(lineSpan.Path);
                        IoC.BuildUp(openDocumentResult);
                        openDocumentResult.Execute(null);
                    });
            }
        }

        private static ErrorListItemType GetItemType(DiagnosticSeverity severity)
        {
            switch (severity)
            {
                case DiagnosticSeverity.Info:
                    return ErrorListItemType.Message;
                case DiagnosticSeverity.Warning:
                    return ErrorListItemType.Warning;
                case DiagnosticSeverity.Error:
                    return ErrorListItemType.Error;
                default:
                    throw new ArgumentOutOfRangeException("severity");
            }
        }
    }
}