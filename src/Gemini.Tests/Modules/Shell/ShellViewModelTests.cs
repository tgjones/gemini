using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Framework.Themes;
using Gemini.Modules.Shell.Services;
using Gemini.Modules.Shell.ViewModels;
using Gemini.Modules.Shell.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = System.Action;

namespace Gemini.Tests.Modules.Shell
{
    [STATestClass]
    public class ShellViewModelTests
    {
        [TestMethod]
        [Timeout(10000)]
        public async Task OpenDocumentAsync_NewDocument_RaisesOrderedEventsAndEchoesActiveLayoutItem()
        {
            var shell = new TestShellViewModel();
            await ActivateShellAsync(shell);
            var document = new TestDocument();
            var markers = new List<string>();
            var changingCount = 0;
            var changedCount = 0;
            object changingSender = null;
            object changedSender = null;

            shell.ActiveDocumentChanging += delegate(object sender, EventArgs eventArgs)
            {
                changingCount++;
                changingSender = sender;
                markers.Add("ActiveDocumentChanging");
            };
            shell.ActiveDocumentChanged += delegate(object sender, EventArgs eventArgs)
            {
                changedCount++;
                changedSender = sender;
                markers.Add("ActiveDocumentChanged");
            };

            await shell.OpenDocumentAsync(document);

            CollectionAssert.AreEqual(
                new[] { "ActiveDocumentChanging", "ActiveDocumentChanged" },
                markers);
            Assert.AreEqual(1, changingCount);
            Assert.AreEqual(1, changedCount);
            Assert.AreSame(shell, changingSender);
            Assert.AreSame(shell, changedSender);
            Assert.AreSame(document, shell.ActiveItem);
            Assert.AreSame(document, shell.ActiveLayoutItem);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task OpenDocumentAsync_OverlappingRequests_ExecutesLifecycleInFifoOrder()
        {
            var shell = new TestShellViewModel();
            await ActivateShellAsync(shell);
            var markers = new List<string>();
            var first = new GatedDocument("first", markers);
            var second = new GatedDocument("second", markers);

            var firstTransition = shell.OpenDocumentAsync(first);
            await first.ActivationEntered;

            var secondTransition = shell.OpenDocumentAsync(second);
            Assert.IsFalse(second.ActivationEntered.IsCompleted);

            first.ReleaseActivation();
            await second.ActivationEntered;
            second.ReleaseActivation();
            await Task.WhenAll(firstTransition, secondTransition);

            CollectionAssert.AreEqual(
                new[] { "first-enter", "first-exit", "second-enter", "second-exit" },
                markers);
            Assert.AreSame(second, shell.ActiveItem);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task OpenDocumentAsync_NestedToolTransition_CompletesWithoutDeadlock()
        {
            var shell = new TestShellViewModel();
            await ActivateShellAsync(shell);
            var tool = new TestTool();
            var document = new NestedTransitionDocument(shell, tool);

            await shell.OpenDocumentAsync(document);

            Assert.AreEqual(1, document.ActivationCount);
            Assert.AreEqual(1, tool.ActivationCount);
            Assert.IsTrue(tool.IsVisible);
            Assert.AreSame(tool, shell.Tools.Single());
            Assert.AreSame(document, shell.ActiveItem);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ActiveLayoutItem_BindingEcho_DoesNotDuplicateTransitionOrEvents()
        {
            var shell = new TestShellViewModel();
            await ActivateShellAsync(shell);
            var document = new TestDocument();
            var changingCount = 0;
            var changedCount = 0;

            shell.ActiveDocumentChanging += delegate
            {
                changingCount++;
            };
            shell.ActiveDocumentChanged += delegate
            {
                changedCount++;
            };
            shell.PropertyChanged += delegate(object sender, System.ComponentModel.PropertyChangedEventArgs eventArgs)
            {
                if (eventArgs.PropertyName == nameof(ShellViewModel.ActiveLayoutItem))
                    shell.ActiveLayoutItem = shell.ActiveLayoutItem;
            };

            shell.ActiveLayoutItem = document;

            Assert.AreEqual(1, shell.ActivateItemCallCount);
            Assert.AreEqual(1, changingCount);
            Assert.AreEqual(1, changedCount);
            Assert.AreSame(document, shell.ActiveItem);
        }

        [TestMethod]
        public void OnActivationProcessed_FailedActivation_DoesNotPublishActiveLayoutItem()
        {
            var shell = new TestShellViewModel();
            var document = new TestDocument();

            shell.CompleteActivation(document, false);

            Assert.IsNull(shell.ActiveLayoutItem);

            shell.CompleteActivation(document, true);

            Assert.AreSame(document, shell.ActiveLayoutItem);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ActiveLayoutItem_CurrentItemBehindQueuedTransition_IsNotDropped()
        {
            var shell = new TestShellViewModel();
            await ActivateShellAsync(shell);
            var first = new ReactivatingDocument();
            var second = new TestDocument();
            var tool = new GatedTool();
            await shell.OpenDocumentAsync(first);

            var toolTransition = shell.ShowToolAsync(tool);
            await tool.ActivationEntered;
            var secondTransition = shell.OpenDocumentAsync(second);

            Assert.AreSame(first, shell.ActiveLayoutItem);
            shell.ActiveLayoutItem = first;
            tool.ReleaseActivation();

            await first.SecondActivationEntered;
            await Task.WhenAll(toolTransition, secondTransition);

            Assert.AreEqual(2, first.ActivationCount);
            Assert.AreSame(first, shell.ActiveItem);
            Assert.AreSame(first, shell.ActiveLayoutItem);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task OpenDocumentAsync_ActivationFaultIsObservable_AndLaterRequestStillRuns()
        {
            var shell = new TestShellViewModel();
            await ActivateShellAsync(shell);
            var expectedException = new InvalidOperationException("activation failed");
            var failing = new GatedFaultingDocument(expectedException);
            var succeeding = new TestDocument();

            var failingTransition = shell.OpenDocumentAsync(failing);
            await failing.ActivationEntered;
            var succeedingTransition = shell.OpenDocumentAsync(succeeding);

            failing.ReleaseActivation();
            var actualException = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () => await failingTransition);
            await succeedingTransition;

            Assert.AreSame(expectedException, actualException);
            Assert.AreSame(succeeding, shell.ActiveItem);
            Assert.AreEqual(1, succeeding.ActivationCount);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShowToolAsync_RepeatedRequest_RegistersActivatesAndShowsOnce()
        {
            var shell = new TestShellViewModel();
            await ActivateShellAsync(shell);
            var tool = new TestTool
            {
                IsVisible = false,
                IsSelected = false
            };

            await shell.ShowToolAsync(tool);
            await shell.ShowToolAsync(tool);

            Assert.AreEqual(1, shell.Tools.Count);
            Assert.AreSame(tool, shell.Tools[0]);
            Assert.AreEqual(1, tool.ActivationCount);
            Assert.IsTrue(tool.IsVisible);
            Assert.IsTrue(tool.IsSelected);
            Assert.AreSame(tool, shell.ActiveLayoutItem);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task InitializationTask_WaitsForPostInitializeAsync_AndRunsOnlyOnce()
        {
            var markers = new List<string>();
            var postEntered = new TaskCompletionSource<object>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            var postGate = new TaskCompletionSource<object>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            var module = new TestModule(
                () => markers.Add("pre"),
                () => markers.Add("initialize"),
                async () =>
                {
                    markers.Add("post-enter");
                    postEntered.TrySetResult(null);
                    await postGate.Task;
                    markers.Add("post-exit");
                });
            var shell = CreateInitializationShell(
                new[] { module },
                TestThemeManager.WithCurrentTheme(),
                new TestStatePersister(() => false));
            await ActivateShellAsync(shell);

            shell.LoadView(new TestShellView());
            await postEntered.Task;

            Assert.IsFalse(shell.InitializationTask.IsCompleted);
            postGate.TrySetResult(null);
            await shell.InitializationTask;
            shell.LoadView(new TestShellView());

            CollectionAssert.AreEqual(
                new[] { "pre", "initialize", "post-enter", "post-exit" },
                markers);
            Assert.AreEqual(1, module.PreInitializeCallCount);
            Assert.AreEqual(1, module.InitializeCallCount);
            Assert.AreEqual(1, module.PostInitializeCallCount);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task InitializationTask_ModuleFailure_IsObservable()
        {
            var expectedException = new InvalidOperationException("module failed");
            var module = new TestModule(
                () => throw expectedException,
                () => { },
                () => Task.CompletedTask);
            var shell = CreateInitializationShell(
                new[] { module },
                TestThemeManager.WithCurrentTheme(),
                new TestStatePersister(() => false));
            await ActivateShellAsync(shell);

            shell.LoadView(new TestShellView());
            var actualException = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () => await shell.InitializationTask);

            Assert.AreSame(expectedException, actualException);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task InitializationTask_ThemeFailure_IsObservable()
        {
            var themeManager = TestThemeManager.WithoutLoadableTheme();
            var shell = CreateInitializationShell(
                new[] { TestModule.Empty },
                themeManager,
                new TestStatePersister(() => false));
            await ActivateShellAsync(shell);

            shell.LoadView(new TestShellView());
            var exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () => await shell.InitializationTask);

            Assert.AreEqual("unable to load application theme", exception.Message);
            Assert.AreEqual(2, themeManager.SetCurrentThemeCallCount);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task InitializationTask_RestoreFailure_IsObservable()
        {
            var expectedException = new InvalidOperationException("restore failed");
            var shell = CreateInitializationShell(
                new[] { TestModule.Empty },
                TestThemeManager.WithCurrentTheme(),
                new TestStatePersister(() => throw expectedException));
            await ActivateShellAsync(shell);

            shell.LoadView(new TestShellView());
            var actualException = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () => await shell.InitializationTask);

            Assert.AreSame(expectedException, actualException);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task InitializationTask_UnreadablePersistedState_FallsBackToDefaults()
        {
            var shell = CreateInitializationShell(
                new[] { TestModule.Empty },
                TestThemeManager.WithCurrentTheme(),
                new TestStatePersister(() => false));
            File.WriteAllText(shell.StateFile, "invalid state");

            try
            {
                await ActivateShellAsync(shell);

                shell.LoadView(new TestShellView());
                await shell.InitializationTask;
            }
            finally
            {
                File.Delete(shell.StateFile);
            }
        }

        private static TestShellViewModel CreateInitializationShell(
            IEnumerable<IModule> modules,
            IThemeManager themeManager,
            ILayoutItemStatePersister statePersister)
        {
            var shell = new TestShellViewModel();
            SetPrivateField(shell, "_modules", modules);
            SetPrivateField(shell, "_themeManager", themeManager);
            SetPrivateField(shell, "_layoutItemStatePersister", statePersister);
            return shell;
        }

        private static void SetPrivateField<T>(ShellViewModel shell, string fieldName, T value)
        {
            var field = typeof(ShellViewModel).GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field);
            field.SetValue(shell, value);
        }

        private static Task ActivateShellAsync(ShellViewModel shell)
        {
            return ((IActivate)shell).ActivateAsync(CancellationToken.None);
        }

        private sealed class TestShellViewModel : ShellViewModel
        {
            private readonly string _stateFile = Path.Combine(
                Path.GetTempPath(),
                "gemini-shell-test-" + Guid.NewGuid().ToString("N") + ".bin");

            public int ActivateItemCallCount { get; private set; }

            public override string StateFile => _stateFile;

            public override Task ActivateItemAsync(
                IDocument item,
                CancellationToken cancellationToken)
            {
                ActivateItemCallCount++;
                return base.ActivateItemAsync(item, cancellationToken);
            }

            public void LoadView(IShellView view)
            {
                OnViewLoaded(view);
            }

            public void CompleteActivation(IDocument document, bool success)
            {
                OnActivationProcessed(document, success);
            }
        }

        private class TestDocument : Document
        {
            public TestDocument()
            {
                DisplayName = "Test document";
            }

            public int ActivationCount { get; private set; }

            protected override Task OnActivatedAsync(CancellationToken cancellationToken)
            {
                ActivationCount++;
                return base.OnActivatedAsync(cancellationToken);
            }
        }

        private sealed class NestedTransitionDocument : Document
        {
            private readonly IShell _shell;
            private readonly ITool _tool;

            public NestedTransitionDocument(IShell shell, ITool tool)
            {
                _shell = shell;
                _tool = tool;
            }

            public int ActivationCount { get; private set; }

            protected override async Task OnActivatedAsync(CancellationToken cancellationToken)
            {
                ActivationCount++;
                await _shell.ShowToolAsync(_tool);
                await base.OnActivatedAsync(cancellationToken);
            }
        }
        private sealed class ReactivatingDocument : TestDocument
        {
            private readonly TaskCompletionSource<object> _secondActivationEntered =
                new TaskCompletionSource<object>(
                    TaskCreationOptions.RunContinuationsAsynchronously);

            public Task SecondActivationEntered => _secondActivationEntered.Task;

            protected override async Task OnActivatedAsync(CancellationToken cancellationToken)
            {
                await base.OnActivatedAsync(cancellationToken);
                if (ActivationCount == 2)
                    _secondActivationEntered.TrySetResult(null);
            }
        }

        private sealed class GatedDocument : TestDocument
        {
            private readonly string _name;
            private readonly IList<string> _markers;
            private readonly TaskCompletionSource<object> _activationEntered;
            private readonly TaskCompletionSource<object> _activationGate;

            public GatedDocument(string name, IList<string> markers)
            {
                _name = name;
                _markers = markers;
                _activationEntered = new TaskCompletionSource<object>(
                    TaskCreationOptions.RunContinuationsAsynchronously);
                _activationGate = new TaskCompletionSource<object>(
                    TaskCreationOptions.RunContinuationsAsynchronously);
            }

            public Task ActivationEntered => _activationEntered.Task;

            public void ReleaseActivation()
            {
                _activationGate.TrySetResult(null);
            }

            protected override async Task OnActivatedAsync(CancellationToken cancellationToken)
            {
                _markers.Add(_name + "-enter");
                _activationEntered.TrySetResult(null);
                await _activationGate.Task;
                _markers.Add(_name + "-exit");
                await base.OnActivatedAsync(cancellationToken);
            }
        }

        private sealed class GatedFaultingDocument : Document
        {
            private readonly Exception _exception;
            private readonly TaskCompletionSource<object> _activationEntered;
            private readonly TaskCompletionSource<object> _activationGate;

            public GatedFaultingDocument(Exception exception)
            {
                _exception = exception;
                _activationEntered = new TaskCompletionSource<object>(
                    TaskCreationOptions.RunContinuationsAsynchronously);
                _activationGate = new TaskCompletionSource<object>(
                    TaskCreationOptions.RunContinuationsAsynchronously);
            }

            public Task ActivationEntered => _activationEntered.Task;

            public void ReleaseActivation()
            {
                _activationGate.TrySetResult(null);
            }

            protected override async Task OnActivatedAsync(CancellationToken cancellationToken)
            {
                _activationEntered.TrySetResult(null);
                await _activationGate.Task;
                throw _exception;
            }
        }

        private sealed class TestTool : Tool
        {
            public override PaneLocation PreferredLocation => PaneLocation.Left;

            public int ActivationCount { get; private set; }

            protected override Task OnActivatedAsync(CancellationToken cancellationToken)
            {
                ActivationCount++;
                return base.OnActivatedAsync(cancellationToken);
            }
        }

        private sealed class GatedTool : Tool
        {
            private readonly TaskCompletionSource<object> _activationEntered =
                new TaskCompletionSource<object>(
                    TaskCreationOptions.RunContinuationsAsynchronously);
            private readonly TaskCompletionSource<object> _activationGate =
                new TaskCompletionSource<object>(
                    TaskCreationOptions.RunContinuationsAsynchronously);

            public override PaneLocation PreferredLocation => PaneLocation.Left;

            public Task ActivationEntered => _activationEntered.Task;

            public void ReleaseActivation()
            {
                _activationGate.TrySetResult(null);
            }

            protected override async Task OnActivatedAsync(CancellationToken cancellationToken)
            {
                _activationEntered.TrySetResult(null);
                await _activationGate.Task;
                await base.OnActivatedAsync(cancellationToken);
            }
        }

        private sealed class TestModule : IModule
        {
            private readonly Action _preInitialize;
            private readonly Action _initialize;
            private readonly Func<Task> _postInitialize;

            public static TestModule Empty => new TestModule(
                () => { },
                () => { },
                () => Task.CompletedTask);

            public TestModule(
                Action preInitialize,
                Action initialize,
                Func<Task> postInitialize)
            {
                _preInitialize = preInitialize;
                _initialize = initialize;
                _postInitialize = postInitialize;
            }

            public IEnumerable<ResourceDictionary> GlobalResourceDictionaries
                => Enumerable.Empty<ResourceDictionary>();

            public IEnumerable<IDocument> DefaultDocuments
                => Enumerable.Empty<IDocument>();

            public IEnumerable<Type> DefaultTools
                => Enumerable.Empty<Type>();

            public int PreInitializeCallCount { get; private set; }

            public int InitializeCallCount { get; private set; }

            public int PostInitializeCallCount { get; private set; }

            public void PreInitialize()
            {
                PreInitializeCallCount++;
                _preInitialize();
            }

            public void Initialize()
            {
                InitializeCallCount++;
                _initialize();
            }

            public Task PostInitializeAsync()
            {
                PostInitializeCallCount++;
                return _postInitialize();
            }
        }

        private sealed class TestThemeManager : IThemeManager
        {
            private readonly bool _canSetTheme;

            private TestThemeManager(ITheme currentTheme, bool canSetTheme)
            {
                CurrentTheme = currentTheme;
                _canSetTheme = canSetTheme;
                Themes = new List<ITheme>();
            }

            public event EventHandler CurrentThemeChanged
            {
                add { }
                remove { }
            }

            public List<ITheme> Themes { get; }

            public ITheme CurrentTheme { get; private set; }

            public int SetCurrentThemeCallCount { get; private set; }

            public static TestThemeManager WithCurrentTheme()
            {
                return new TestThemeManager(new TestTheme(), true);
            }

            public static TestThemeManager WithoutLoadableTheme()
            {
                return new TestThemeManager(null, false);
            }

            public bool SetCurrentTheme(string name)
            {
                SetCurrentThemeCallCount++;
                if (!_canSetTheme)
                    return false;

                CurrentTheme = new TestTheme();
                return true;
            }
        }

        private sealed class TestTheme : ITheme
        {
            public string Name => "Test";

            public IEnumerable<Uri> ApplicationResources => Enumerable.Empty<Uri>();

            public IEnumerable<Uri> MainWindowResources => Enumerable.Empty<Uri>();
        }

        private sealed class TestStatePersister : ILayoutItemStatePersister
        {
            private readonly Func<bool> _loadState;

            public TestStatePersister(Func<bool> loadState)
            {
                _loadState = loadState;
            }

            public bool SaveState(IShell shell, IShellView shellView, string fileName)
            {
                return true;
            }

            public bool LoadState(IShell shell, IShellView shellView, string fileName)
            {
                return _loadState();
            }
        }

        private sealed class TestShellView : IShellView
        {
            public void LoadLayout(
                Stream stream,
                Action<ITool> addToolCallback,
                Action<IDocument> addDocumentCallback,
                Dictionary<string, ILayoutItem> itemsState)
            {
            }

            public void SaveLayout(Stream stream)
            {
            }

            public void UpdateFloatingWindows()
            {
            }
        }
    }
}
