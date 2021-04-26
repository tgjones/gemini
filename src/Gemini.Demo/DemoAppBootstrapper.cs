using System.Collections.Generic;
using System.Reflection;

namespace Gemini.Demo
{
    /// <summary>
    /// Only needed when you want Gemini.Demo to function under .NET5+ with PublishSingleFile
    /// </summary>
    public class DemoAppBootstrapper : Gemini.AppBootstrapper
    {
        public override bool IsPublishSingleFileHandled => true;

        protected override IEnumerable<Assembly> PublishSingleFileBypassAssemblies
        {
            get
            {
                yield return Assembly.GetAssembly(typeof(Gemini.AppBootstrapper)); // GeminiWpf
                yield return Assembly.GetAssembly(typeof(Gemini.Modules.CodeCompiler.ICodeCompiler));
                yield return Assembly.GetAssembly(typeof(Gemini.Modules.CodeEditor.ILanguageDefinition));
                yield return Assembly.GetAssembly(typeof(Gemini.Modules.ErrorList.IErrorList));
                yield return Assembly.GetAssembly(typeof(Gemini.Modules.GraphEditor.Module));
                yield return Assembly.GetAssembly(typeof(Gemini.Modules.Inspector.IInspectorTool));
                yield return Assembly.GetAssembly(typeof(Gemini.Modules.Output.IOutput));
                yield return Assembly.GetAssembly(typeof(Gemini.Modules.PropertyGrid.IPropertyGrid));
                // add more assemblies with exports as needed here
            }
        }
    };
}
