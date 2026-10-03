using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework;

namespace Caliburn5PackageHost
{
    internal static class Program
    {
        [STAThread]
        private static int Main()
        {
            var extension = new LifecycleExtension();
            extension.Activated += extension.OnActivatedAsync;

            return typeof(WindowBase).Assembly.GetName().Name == "Gemini"
                && typeof(Screen).Assembly.GetName().Name == "Caliburn.Micro.Core"
                && typeof(BootstrapperBase).Assembly.GetName().Name == "Caliburn.Micro.Platform"
                ? 0
                : 1;
        }

        private sealed class LifecycleExtension : WindowBase
        {
            public Task OnActivatedAsync(object sender, ActivationEventArgs eventArgs)
                => Task.CompletedTask;
        }
    }
}
