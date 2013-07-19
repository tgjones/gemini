using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace Gemini
{
	public class AppBootstrapper : Bootstrapper<IMainWindow>
	{
		private CompositionContainer _container;

		/// <summary>
		/// By default, we are configured to use MEF
		/// </summary>
		protected override void Configure()
		{
            // Add all assemblies to AssemblySource (using a temporary DirectoryCatalog).
            var directoryCatalog = new DirectoryCatalog(@"./");
            AssemblySource.Instance.AddRange(
                directoryCatalog.Parts
                    .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly)
                    .Where(assembly => !AssemblySource.Instance.Contains(assembly)));

            // Prioritise the executable assembly. This allows the client project to override exports, including IShell.
            // The client project can override SelectAssemblies to choose which assemblies are prioritised.
		    var priorityAssemblies = SelectAssemblies().ToList();
		    var priorityCatalog = new AggregateCatalog(priorityAssemblies.Select(x => new AssemblyCatalog(x)));
		    var priorityProvider = new CatalogExportProvider(priorityCatalog);

            // Now get all other assemblies (excluding the priority assemblies).
			var mainCatalog = new AggregateCatalog(
                AssemblySource.Instance
                    .Where(assembly => !priorityAssemblies.Contains(assembly))
                    .Select(x => new AssemblyCatalog(x)));
		    var mainProvider = new CatalogExportProvider(mainCatalog);

			_container = new CompositionContainer(priorityProvider, mainProvider);
		    priorityProvider.SourceProvider = _container;
		    mainProvider.SourceProvider = _container;

			var batch = new CompositionBatch();

		    BindServices(batch);
            batch.AddExportedValue(mainCatalog);

			_container.Compose(batch);
		}

	    protected virtual void BindServices(CompositionBatch batch)
        {
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(_container);
        }

		protected override object GetInstance(Type serviceType, string key)
		{
			string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
			var exports = _container.GetExportedValues<object>(contract);

			if (exports.Count() > 0)
				return exports.First();

			throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
		}

		protected override IEnumerable<object> GetAllInstances(Type serviceType)
		{
			return _container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
		}

		protected override void BuildUp(object instance)
		{
			_container.SatisfyImportsOnce(instance);
		}
	}
}