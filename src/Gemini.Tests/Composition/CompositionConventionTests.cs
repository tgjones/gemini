using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Gemini.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gemini.Tests.Composition
{
    [TestClass]
    public class CompositionConventionTests
    {
        [TestMethod]
        public void PriorityProvider_FirstExportOverridesMainProviderForSingleResolution()
        {
            using (var priorityCatalog = new TypeCatalog(typeof(PriorityService)))
            using (var mainCatalog = new TypeCatalog(typeof(DefaultService)))
            using (var priorityProvider = new CatalogExportProvider(priorityCatalog))
            using (var mainProvider = new CatalogExportProvider(mainCatalog))
            using (var container = new CompositionContainer(priorityProvider, mainProvider))
            {
                priorityProvider.SourceProvider = container;
                mainProvider.SourceProvider = container;

                var contract = AttributedModelServices.GetContractName(typeof(ITestService));
                var service = (ITestService)container.GetExports<object>(contract).First().Value;

                Assert.IsInstanceOfType<PriorityService>(service);
            }
        }

        [TestMethod]
        public void NonSharedDocumentExport_CreatesDistinctDocumentInstances()
        {
            using (var container = new CompositionContainer(
                new TypeCatalog(typeof(NonSharedDocument))))
            {
                var first = container.GetExportedValue<NonSharedDocument>();
                var second = container.GetExportedValue<NonSharedDocument>();

                Assert.AreNotSame(first, second);
            }
        }

        [TestMethod]
        public void MissingRequiredImport_RemainsVisibleAsCompositionFailure()
        {
            using (var container = new CompositionContainer(
                new TypeCatalog(typeof(RequiresMissingImport))))
            {
                var exception = Assert.ThrowsExactly<ImportCardinalityMismatchException>(
                    () => container.GetExportedValue<RequiresMissingImport>());

                StringAssert.Contains(exception.Message, typeof(RequiresMissingImport).FullName);
            }
        }

        private interface ITestService
        {
        }

        [Export(typeof(ITestService))]
        private sealed class PriorityService : ITestService
        {
        }

        [Export(typeof(ITestService))]
        private sealed class DefaultService : ITestService
        {
        }

        [Export(typeof(NonSharedDocument))]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        private sealed class NonSharedDocument : Document
        {
        }

        private interface IMissingService
        {
        }

        [Export(typeof(RequiresMissingImport))]
        private sealed class RequiresMissingImport
        {
            [ImportingConstructor]
            public RequiresMissingImport(IMissingService missingService)
            {
                MissingService = missingService;
            }

            public IMissingService MissingService { get; }
        }
    }
}