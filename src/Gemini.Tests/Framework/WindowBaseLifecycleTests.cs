using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gemini.Tests.Framework
{
    [STATestClass]
    public class WindowBaseLifecycleTests
    {
        [TestMethod]
        [Timeout(10000)]
        public async Task ActivateAsync_InitializesBeforeActivatedEvent_AndEventObservesActiveState()
        {
            var markers = new List<string>();
            var activated = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            var window = new LifecycleProbeWindow(markers);
            object eventSender = null;
            var activationCount = 0;
            var wasInitializedInHandler = false;
            var wasActiveInHandler = false;

            window.Activated += delegate(object sender, ActivationEventArgs eventArgs)
            {
                activationCount++;
                eventSender = sender;
                wasInitializedInHandler = window.IsInitialized;
                wasActiveInHandler = window.IsActive;
                markers.Add("Activated");
                activated.TrySetResult(true);
                return Task.CompletedTask;
            };

            await ((IActivate)window).ActivateAsync(CancellationToken.None);
            await activated.Task;

            CollectionAssert.AreEqual(
                new[] { "Initialize", "Activated" },
                markers);
            Assert.AreEqual(1, activationCount);
            Assert.AreSame(window, eventSender);
            Assert.IsTrue(wasInitializedInHandler);
            Assert.IsTrue(wasActiveInHandler);
            Assert.IsTrue(window.IsInitialized);
            Assert.IsTrue(window.IsActive);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task DeactivateAsync_AfterActivation_RaisesAsyncDeactivatedEvent()
        {
            var markers = new List<string>();
            var eventEntered = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            var releaseEvent = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            var window = new LifecycleProbeWindow(markers);

            await ((IActivate)window).ActivateAsync(CancellationToken.None);
            markers.Clear();

            object eventSender = null;
            var deactivationCount = 0;
            var wasClosed = true;
            window.Deactivated += async delegate(object sender, DeactivationEventArgs eventArgs)
            {
                deactivationCount++;
                eventSender = sender;
                wasClosed = eventArgs.WasClosed;
                markers.Add("Deactivated");
                eventEntered.TrySetResult(true);
                await releaseEvent.Task;
            };

            var deactivationTask = ((IDeactivate)window).DeactivateAsync(
                false,
                CancellationToken.None);
            await eventEntered.Task;
            var lifecycleAwaitedEventHandler = !deactivationTask.IsCompleted;
            releaseEvent.TrySetResult(true);
            await deactivationTask;

            CollectionAssert.AreEqual(
                new[] { "Deactivate", "Deactivated" },
                markers);
            Assert.AreEqual(1, deactivationCount);
            Assert.AreSame(window, eventSender);
            Assert.IsTrue(lifecycleAwaitedEventHandler);
            Assert.IsFalse(wasClosed);
            Assert.IsFalse(window.IsActive);
        }

        private class LifecycleProbeWindow : WindowBase
        {
            private readonly IList<string> _markers;

            public LifecycleProbeWindow(IList<string> markers)
            {
                _markers = markers;
            }

            protected override Task OnInitializedAsync(CancellationToken cancellationToken)
            {
                _markers.Add("Initialize");
                return base.OnInitializedAsync(cancellationToken);
            }

            protected override Task OnDeactivateAsync(
                bool close,
                CancellationToken cancellationToken)
            {
                _markers.Add("Deactivate");
                return base.OnDeactivateAsync(close, cancellationToken);
            }
        }
    }
}
