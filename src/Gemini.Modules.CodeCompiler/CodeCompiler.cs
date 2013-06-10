using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Caliburn.Micro;
using Gemini.Framework.Results;
using Gemini.Framework.Services;
using Gemini.Modules.ErrorList;
using Gemini.Modules.Output;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;

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

            var compilation = Compilation.Create(outputName)
                .WithOptions(new CompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(references)
                .AddSyntaxTrees(syntaxTrees);

            var moduleBuilder = AppDomain.CurrentDomain
                .DefineDynamicAssembly(new AssemblyName(outputName),
                    AssemblyBuilderAccess.RunAndCollect)
                .DefineDynamicModule(outputName);

            var result = compilation.Emit(moduleBuilder);

            ProcessResult(result);

            _output.AppendLine("------ Compile finished");

            return moduleBuilder.Assembly;
        }

        private void ProcessResult(ReflectionEmitResult result)
        {
            _errorList.Items.Clear();

            if (!result.Success)
            {
                foreach (var diagnostic in result.Diagnostics)
                {
                    if (!diagnostic.Location.IsInSource)
                        throw new NotSupportedException("Only support source file locations.");

                    var itemType = GetItemType(diagnostic.Info.Severity);
                    var description = diagnostic.Info.GetMessage();
                    var lineSpan = diagnostic.Location.GetLineSpan(false);

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

            if (result.IsUncollectible)
                _errorList.AddItem(ErrorListItemType.Message, "The compiled assembly is not garbage collectible.");
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