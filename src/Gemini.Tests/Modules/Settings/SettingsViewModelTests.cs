using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Modules.Settings;
using Gemini.Modules.Settings.ViewModels;
using Gemini.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gemini.Tests.Modules.Settings
{
    [STATestClass]
    [DoNotParallelize]
    public class SettingsViewModelTests
    {
        [TestMethod]
        [Timeout(10000)]
        public async Task ActivateAsync_AsyncAndLegacyEditors_BuildsHierarchyAndSelectsFirstLeaf()
        {
            var asyncEditor = new TestAsyncSettingsEditor("Async", "General");
            var legacyEditor = new TestLegacySettingsEditor("Legacy", "General");
            var originalGetInstance = IoC.GetInstance;
            var originalGetAllInstances = IoC.GetAllInstances;
            var originalBuildUp = IoC.BuildUp;
            var asyncDiscoveryCount = 0;
            var legacyDiscoveryCount = 0;
            var scope = new IoCOverrideScope(
                delegate(Type serviceType, string key)
                {
                    throw new InvalidOperationException(
                        "Unexpected single-instance request for " + serviceType.FullName + ".");
                },
                delegate(Type serviceType)
                {
                    if (serviceType == typeof(ISettingsEditorAsync))
                    {
                        asyncDiscoveryCount++;
                        return new object[] { asyncEditor };
                    }

                    if (serviceType == typeof(ISettingsEditor))
                    {
                        legacyDiscoveryCount++;
                        return new object[] { legacyEditor };
                    }

                    throw new InvalidOperationException(
                        "Unexpected multi-instance request for " + serviceType.FullName + ".");
                },
                delegate(object instance)
                {
                    throw new InvalidOperationException(
                        "Unexpected build-up request for " + instance.GetType().FullName + ".");
                });

            try
            {
                var viewModel = new SettingsViewModel();

                await ((IActivate)viewModel).ActivateAsync(CancellationToken.None);

                Assert.AreEqual("Options", viewModel.DisplayName);
                Assert.AreEqual(1, asyncDiscoveryCount);
                Assert.AreEqual(1, legacyDiscoveryCount);
                Assert.AreEqual(1, viewModel.Pages.Count);

                var generalPage = viewModel.Pages[0];
                Assert.AreEqual("General", generalPage.Name);
                Assert.AreEqual(0, generalPage.Editors.Count);
                Assert.AreEqual(2, generalPage.Children.Count);
                CollectionAssert.AreEqual(
                    new[] { "Async", "Legacy" },
                    generalPage.Children.Select(page => page.Name).ToArray());

                var asyncPage = generalPage.Children[0];
                var legacyPage = generalPage.Children[1];
                Assert.AreEqual(0, asyncPage.Children.Count);
                Assert.AreEqual(1, asyncPage.Editors.Count);
                Assert.AreSame(asyncEditor, asyncPage.Editors[0]);
                Assert.AreEqual(0, legacyPage.Children.Count);
                Assert.AreEqual(1, legacyPage.Editors.Count);
                Assert.AreSame(legacyEditor, legacyPage.Editors[0]);
                Assert.AreSame(asyncPage, viewModel.SelectedPage);

                var placedEditors = generalPage.Editors
                    .Concat(generalPage.Children.SelectMany(page => page.Editors))
                    .ToList();
                Assert.AreEqual(2, placedEditors.Count);
                Assert.AreEqual(
                    1,
                    placedEditors.Count(editor => ReferenceEquals(editor, asyncEditor)));
                Assert.AreEqual(
                    1,
                    placedEditors.Count(editor => ReferenceEquals(editor, legacyEditor)));
            }
            finally
            {
                scope.Dispose();
                Assert.AreSame(originalGetInstance, IoC.GetInstance);
                Assert.AreSame(originalGetAllInstances, IoC.GetAllInstances);
                Assert.AreSame(originalBuildUp, IoC.BuildUp);
            }
        }

        private sealed class TestAsyncSettingsEditor : ISettingsEditorAsync
        {
            public TestAsyncSettingsEditor(string settingsPageName, string settingsPagePath)
            {
                SettingsPageName = settingsPageName;
                SettingsPagePath = settingsPagePath;
            }

            public string SettingsPageName { get; }

            public string SettingsPagePath { get; }

            public Task ApplyChangesAsync()
            {
                return Task.CompletedTask;
            }
        }

        private sealed class TestLegacySettingsEditor : ISettingsEditor
        {
            public TestLegacySettingsEditor(string settingsPageName, string settingsPagePath)
            {
                SettingsPageName = settingsPageName;
                SettingsPagePath = settingsPagePath;
            }

            public string SettingsPageName { get; }

            public string SettingsPagePath { get; }

            public void ApplyChanges()
            {
            }
        }
    }
}
