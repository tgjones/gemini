using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace Gemini
{
    public class AppBootstrapper : BootstrapperBase
    {
        private List<Assembly> _priorityAssemblies;

        protected CompositionContainer Container { get; set; }

        internal IList<Assembly> PriorityAssemblies
            => _priorityAssemblies;

        /// <summary>
        /// Override this to true if your main application properly handles PublishSingleFile cases.
        /// Otherwise, an exception in <see cref="Configure"/> is thrown if Gemini detects
        /// telltale signs of an PublishSingleFile environment under .NET5+.
        /// </summary>
        public virtual bool IsPublishSingleFileHandled => false;

        public AppBootstrapper()
        {
            PreInitialize();
            Initialize();
        }

        protected virtual void PreInitialize()
        {
            var code = Properties.Settings.Default.LanguageCode;

            if (!string.IsNullOrWhiteSpace(code))
            {
                var culture = CultureInfo.GetCultureInfo(code);
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
            }
        }

        /// <summary>
        /// By default, we are configured to use MEF
        /// </summary>
        protected override void Configure()
        {
            if (CheckIfGeminiAppearsPublishedToSingleFile())
            {
                if (!IsPublishSingleFileHandled)
                {
                    const string fullMethodName =
                        nameof(Gemini) + "." +
                        nameof(AppBootstrapper)+ "."+
                        nameof(Configure);

                    string exceptionMessage =
                        "Gemini appears to be loaded by a .NET5+ app that was deployed with PublishSingleFile (.pubxml), or possibly loaded from memory. " +
                        $"Set {nameof(IsPublishSingleFileHandled)} to true if you expect this and are handling it in your app. " +
                        $"Otherwise, {fullMethodName} and MEF may not find your assemblies with exports.";

                    // Need to show a message, else the program dies without any information to the user
                    if (!System.Diagnostics.Debugger.IsAttached)
                    {
                        MessageBox.Show(exceptionMessage, "GeminiWpf");
                    }

                    var exception = new InvalidOperationException(exceptionMessage)
                    {
                        HelpLink = "https://docs.microsoft.com/en-us/dotnet/core/deploying/single-file",
                    };

                    throw exception;
                }

                // First, add the assemblies which DirectoryCatalog can't find because they are embedded in the app.
                // Unlike the "directoryCatalog.Parts" LINQ below, we don't try to filter out duplicate assemblies here.
                AssemblySource.Instance.AddRange(PublishSingleFileBypassAssemblies);
            }

            // If these paths are different, it suggests this is a .netcoreapp3.1 PublishSingleFile,
            // which extracts files to the Temp directory (AppContext.BaseDirectory).
            // In .NET5+, the files are NOT extracted, unless IncludeAllContentForSelfExtract is set in the .pubxml.
            // See https://docs.microsoft.com/en-us/dotnet/core/deploying/single-file#other-considerations
            string currentWorkingDir = Path.GetDirectoryName(Path.GetFullPath(@"./"));
            string baseDirectory = Path.GetDirectoryName(Path.GetFullPath(AppContext.BaseDirectory));

            // Add all assemblies to AssemblySource (using a temporary DirectoryCatalog).
            PopulateAssemblySourceUsingDirectoryCatalog(currentWorkingDir);
            if (currentWorkingDir != baseDirectory)
            {
                PopulateAssemblySourceUsingDirectoryCatalog(baseDirectory);
            }

            // Prioritise the executable assembly. This allows the client project to override exports, including IShell.
            // The client project can override SelectAssemblies to choose which assemblies are prioritised.
            _priorityAssemblies = SelectAssemblies().ToList();
            var priorityCatalog = new AggregateCatalog(_priorityAssemblies.Select(x => new AssemblyCatalog(x)));
            var priorityProvider = new CatalogExportProvider(priorityCatalog);

            // Now get all other assemblies (excluding the priority assemblies).
            var mainCatalog = new AggregateCatalog(
                AssemblySource.Instance
                    .Where(assembly => !_priorityAssemblies.Contains(assembly))
                    .Select(x => new AssemblyCatalog(x)));
            var mainProvider = new CatalogExportProvider(mainCatalog);

            Container = new CompositionContainer(priorityProvider, mainProvider);
            priorityProvider.SourceProvider = Container;
            mainProvider.SourceProvider = Container;

            var batch = new CompositionBatch();

            BindServices(batch);
            batch.AddExportedValue(mainCatalog);

            Container.Compose(batch);
        }

        protected void PopulateAssemblySourceUsingDirectoryCatalog(string path)
        {
            var directoryCatalog = new DirectoryCatalog(path);
            AssemblySource.Instance.AddRange(
                directoryCatalog.Parts
                    .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly)
                    .Where(assembly => !AssemblySource.Instance.Contains(assembly)));
        }

        /// <summary>
        /// Does a best-guess check to determine if Gemini was deployed under an app with
        /// a PublishSingleFile configuration in .NET5+ environments.
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckIfGeminiAppearsPublishedToSingleFile()
        {
            var geminiAssembly = Assembly.GetAssembly(typeof(Gemini.AppBootstrapper));

            // https://github.com/dotnet/runtime/issues/36590 "Support Single-File Apps in .NET 5"

            // https://github.com/dotnet/designs/blob/main/accepted/2020/single-file/design.md#assemblylocation
            // "Proposed solution is for Assembly.Location to return the empty-string for bundled assemblies, which is the default behavior for assemblies loaded from memory."
            // https://docs.microsoft.com/en-us/dotnet/core/deploying/single-file#api-incompatibility
            // I suppose it may be possible that some .NET obfuscators and "protectors" would lead to this behavior, so this should
            if (geminiAssembly.Location == string.Empty)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// When your application is deployed using PublishSingleFile under .NET5+, override
        /// this to explicitly list the assemblies that MEF needs to search exports for.
        /// </summary>
        protected virtual IEnumerable<Assembly> PublishSingleFileBypassAssemblies
            => Enumerable.Empty<Assembly>();

        protected virtual void BindServices(CompositionBatch batch)
        {
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(Container);
            batch.AddExportedValue(this);
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = Container.GetExports<object>(contract);

            if (exports.Any())
                return exports.First().Value;

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
            => Container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));

        protected override void BuildUp(object instance)
            => Container.SatisfyImportsOnce(instance);

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            DisplayRootViewFor<IMainWindow>();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
            => new[] { Assembly.GetEntryAssembly() };
    };
}
