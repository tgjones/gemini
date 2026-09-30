using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.ViewModels;
using Gemini.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gemini.Tests.Framework
{
    [STATestClass]
    public class ToolCloseTests
    {
        [TestMethod]
        [Timeout(10000)]
        public async Task TryCloseAsync_WhenGuardVetoes_KeepsToolVisibleAndActive()
        {
            var tool = new GuardedTool { CanClose = false };
            var shell = await ShowInShellAsync(tool);

            using (UseShell(shell))
                await tool.TryCloseAsync();

            Assert.AreEqual(1, tool.CanCloseCallCount);
            Assert.IsTrue(tool.IsVisible);
            Assert.IsTrue(tool.IsActive);
            Assert.AreSame(tool, shell.ActiveLayoutItem);
            Assert.AreSame(tool, shell.Tools[0]);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task TryCloseAsync_WhenCloseSucceeds_HidesToolAfterClosedDeactivation()
        {
            var tool = new GuardedTool { CanClose = true };
            var shell = await ShowInShellAsync(tool);

            using (UseShell(shell))
                await tool.TryCloseAsync();

            Assert.AreEqual(1, tool.CanCloseCallCount);
            Assert.IsFalse(tool.IsVisible);
            Assert.IsFalse(tool.IsActive);
            Assert.IsFalse(tool.IsSelected);
            Assert.IsNull(shell.ActiveLayoutItem);
            Assert.AreSame(tool, shell.Tools[0]);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShowToolAsync_AfterSuccessfulClose_ReopensAndReactivatesTool()
        {
            var tool = new GuardedTool { CanClose = true };
            var shell = await ShowInShellAsync(tool);

            using (UseShell(shell))
                await tool.TryCloseAsync();

            await shell.ShowToolAsync(tool);

            Assert.IsTrue(tool.IsVisible);
            Assert.IsTrue(tool.IsActive);
            Assert.IsTrue(tool.IsSelected);
            Assert.AreSame(tool, shell.ActiveLayoutItem);
            Assert.AreSame(tool, shell.Tools[0]);
            Assert.AreEqual(2, tool.ActivationCount);
        }

        private static async Task<ShellViewModel> ShowInShellAsync(Tool tool)
        {
            var shell = new ShellViewModel();
            await ((IActivate)shell).ActivateAsync(CancellationToken.None);
            await shell.ShowToolAsync(tool);
            return shell;
        }

        private static IoCOverrideScope UseShell(IShell shell)
        {
            return new IoCOverrideScope(
                delegate(Type serviceType, string key)
                {
                    if (serviceType == typeof(IShell))
                        return shell;

                    throw new InvalidOperationException(
                        "Unexpected service request for " + serviceType.FullName + ".");
                },
                delegate(Type serviceType)
                {
                    throw new InvalidOperationException(
                        "Unexpected service collection request for " + serviceType.FullName + ".");
                },
                delegate(object instance)
                {
                    throw new InvalidOperationException(
                        "Unexpected build-up request for " + instance.GetType().FullName + ".");
                });
        }

        private sealed class GuardedTool : Tool
        {
            public override PaneLocation PreferredLocation => PaneLocation.Left;

            public bool CanClose { get; set; }

            public int CanCloseCallCount { get; private set; }

            public int ActivationCount { get; private set; }

            public override Task<bool> CanCloseAsync(CancellationToken cancellationToken)
            {
                CanCloseCallCount++;
                return Task.FromResult(CanClose);
            }

            protected override Task OnActivatedAsync(CancellationToken cancellationToken)
            {
                ActivationCount++;
                return base.OnActivatedAsync(cancellationToken);
            }
        }
    }
}