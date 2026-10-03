using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace Gemini.Tests.Framework.Results
{

    internal sealed class QueuedSynchronizationContext : SynchronizationContext
    {
        private readonly ConcurrentQueue<Tuple<SendOrPostCallback, object>> _callbacks =
            new ConcurrentQueue<Tuple<SendOrPostCallback, object>>();
        private readonly TaskCompletionSource<object> _posted =
            new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        private int _postCount;

        internal Task Posted
        {
            get { return _posted.Task; }
        }

        internal int PostCount
        {
            get { return Volatile.Read(ref _postCount); }
        }

        public override void Post(SendOrPostCallback callback, object state)
        {
            _callbacks.Enqueue(Tuple.Create(callback, state));
            Interlocked.Increment(ref _postCount);
            _posted.TrySetResult(null);
        }

        internal void ExecuteNext()
        {
            Tuple<SendOrPostCallback, object> callback;
            if (!_callbacks.TryDequeue(out callback))
                throw new InvalidOperationException("No synchronization callback is queued.");

            callback.Item1(callback.Item2);
        }
    }

    internal sealed class CompletionCapture
    {
        private readonly TaskCompletionSource<ResultCompletionEventArgs> _first =
            new TaskCompletionSource<ResultCompletionEventArgs>(
                TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource<ResultCompletionEventArgs> _second =
            new TaskCompletionSource<ResultCompletionEventArgs>(
                TaskCreationOptions.RunContinuationsAsynchronously);
        private int _count;

        internal CompletionCapture(IResult result)
        {
            result.Completed += delegate(object sender, ResultCompletionEventArgs eventArgs)
            {
                Sender = sender;
                Last = eventArgs;
                CallbackThreadId = Environment.CurrentManagedThreadId;
                var count = Interlocked.Increment(ref _count);
                if (count == 1)
                    _first.TrySetResult(eventArgs);
                else if (count == 2)
                    _second.TrySetResult(eventArgs);
            };
        }

        internal int Count
        {
            get { return Volatile.Read(ref _count); }
        }

        internal object Sender { get; private set; }

        internal ResultCompletionEventArgs Last { get; private set; }

        internal int CallbackThreadId { get; private set; }

        internal Task<ResultCompletionEventArgs> First
        {
            get { return _first.Task; }
        }

        internal Task<ResultCompletionEventArgs> Second
        {
            get { return _second.Task; }
        }
    }

    internal static class ResultTestComposition
    {
        internal static CompositionContainer Compose(
            object result,
            IShell shell,
            IEditorOpeningService editorOpeningService = null)
        {
            var container = new CompositionContainer();
            var batch = new CompositionBatch();
            AttributedModelServices.AddExportedValue(batch, shell);
            AttributedModelServices.AddExportedValue(
                batch,
                editorOpeningService ?? new UnexpectedEditorOpeningService());
            container.Compose(batch);
            AttributedModelServices.SatisfyImportsOnce(container, result);
            return container;
        }

        private sealed class UnexpectedEditorOpeningService : IEditorOpeningService
        {
            public Task<Gemini.Framework.IDocument> OpenFileAsync(
                string path,
                Action<Gemini.Framework.IDocument> configure = null)
            {
                throw new InvalidOperationException("Unexpected editor opening service call.");
            }

            public Task<Gemini.Framework.IDocument> NewFileAsync(
                IEditorProvider editorProvider,
                string name)
            {
                throw new InvalidOperationException("Unexpected editor opening service call.");
            }
        }
    }

    internal sealed class ControlledShell : Screen, IShell
    {
        internal ControlledShell()
        {
            OpenDocumentTask = Task.CompletedTask;
            ShowToolTask = Task.CompletedTask;
        }

        public event EventHandler ActiveDocumentChanging
        {
            add { }
            remove { }
        }

        public event EventHandler ActiveDocumentChanged
        {
            add { }
            remove { }
        }

        public bool ShowFloatingWindowsInTaskbar { get; set; }

        public Gemini.Modules.MainMenu.IMenu MainMenu
        {
            get { return null; }
        }

        public Gemini.Modules.ToolBars.IToolBars ToolBars
        {
            get { return null; }
        }

        public Gemini.Modules.StatusBar.IStatusBar StatusBar
        {
            get { return null; }
        }

        public Gemini.Framework.ILayoutItem ActiveLayoutItem { get; set; }

        public Gemini.Framework.IDocument ActiveItem
        {
            get { return ActiveLayoutItem as Gemini.Framework.IDocument; }
        }

        public IObservableCollection<Gemini.Framework.IDocument> Documents
        {
            get { return null; }
        }

        public IObservableCollection<Gemini.Framework.ITool> Tools
        {
            get { return null; }
        }

        public Task InitializationTask
        {
            get { return Task.CompletedTask; }
        }

        internal Task OpenDocumentTask { get; set; }

        internal Task ShowToolTask { get; set; }

        internal Exception OpenDocumentException { get; set; }

        internal Exception ShowToolException { get; set; }

        internal int OpenDocumentCallCount { get; private set; }

        internal int ShowToolCallCount { get; private set; }

        public bool RegisterTool(Gemini.Framework.ITool tool)
        {
            return false;
        }

        public Task ShowToolAsync<TTool>()
            where TTool : Gemini.Framework.ITool
        {
            return ShowToolAsync(default(TTool));
        }

        public Task ShowToolAsync(Gemini.Framework.ITool model)
        {
            ShowToolCallCount++;
            if (ShowToolException != null)
                throw ShowToolException;

            return ShowToolTask;
        }

        public Task CloseToolAsync(Gemini.Framework.ITool tool)
        {
            return Task.CompletedTask;
        }

        public Task OpenDocumentAsync(Gemini.Framework.IDocument model)
        {
            OpenDocumentCallCount++;
            if (OpenDocumentException != null)
                throw OpenDocumentException;

            return OpenDocumentTask;
        }

        public Task CloseDocumentAsync(Gemini.Framework.IDocument document)
        {
            return Task.CompletedTask;
        }

        public void Close()
        {
        }
    }

    internal sealed class ControlledWindowManager : IWindowManager
    {
        internal ControlledWindowManager()
        {
            ShowWindowTask = Task.CompletedTask;
            ShowDialogTask = Task.FromResult<bool?>(true);
        }

        internal Task ShowWindowTask { get; set; }

        internal Task<bool?> ShowDialogTask { get; set; }

        internal Exception ShowWindowException { get; set; }

        internal Exception ShowDialogException { get; set; }

        internal int ShowWindowCallCount { get; private set; }

        internal int ShowDialogCallCount { get; private set; }

        public Task<bool?> ShowDialogAsync(
            object rootModel,
            object context = null,
            IDictionary<string, object> settings = null)
        {
            ShowDialogCallCount++;
            if (ShowDialogException != null)
                throw ShowDialogException;

            return ShowDialogTask;
        }

        public Task ShowWindowAsync(
            object rootModel,
            object context = null,
            IDictionary<string, object> settings = null)
        {
            ShowWindowCallCount++;
            if (ShowWindowException != null)
                throw ShowWindowException;

            return ShowWindowTask;
        }

        public Task ShowPopupAsync(
            object rootModel,
            object context = null,
            IDictionary<string, object> settings = null)
        {
            throw new InvalidOperationException("Unexpected popup call.");
        }
    }

    internal sealed class CompletionTestDocument : Gemini.Framework.Document
    {
    }

    internal sealed class CompletionTestTool : Gemini.Framework.Tool
    {
        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Left; }
        }
    }

    internal sealed class CompletionTestWindow : Gemini.Framework.WindowBase
    {
    }

    internal static class ResultTestLifecycle
    {
        internal static async Task CloseAsync(object target)
        {
            await ((IActivate)target).ActivateAsync(CancellationToken.None);
            await ((IDeactivate)target).DeactivateAsync(true, CancellationToken.None);
        }
    }
}
