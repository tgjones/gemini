using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Gemini.Modules.Shell.Services
{
    [Export(typeof(IEditorOpeningService))]
    public sealed class EditorOpeningService : IEditorOpeningService
    {
        private readonly IShell _shell;
        private readonly IEditorProvider[] _editorProviders;

        [ImportingConstructor]
        public EditorOpeningService(
            IShell shell,
            [ImportMany] IEditorProvider[] editorProviders)
        {
            _shell = shell;
            _editorProviders = editorProviders;
        }

        /// <inheritdoc />
        public async Task<IDocument> OpenFileAsync(
            string path,
            Action<IDocument> configure = null)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (path.Length == 0)
                throw new ArgumentException("An editor path is required.", nameof(path));

            var existingDocument = FindOpenDocument(path);
            if (existingDocument != null)
            {
                configure?.Invoke(existingDocument);
                var existingPresentation = _shell.OpenDocumentAsync(existingDocument);
                if (existingPresentation == null)
                    throw new InvalidOperationException("The shell returned no document presentation task.");

                await existingPresentation;
                return existingDocument;
            }

            var editorProvider = _editorProviders.FirstOrDefault(x => x.Handles(path));
            if (editorProvider == null)
                throw new NotSupportedException(
                    string.Format("No editor provider handles the path '{0}'.", path));

            return await PresentAndInitializeAsync(
                editorProvider,
                configure,
                document => editorProvider.Open(document, path));
        }

        /// <inheritdoc />
        public async Task<IDocument> NewFileAsync(
            IEditorProvider editorProvider,
            string name)
        {
            if (editorProvider == null)
                throw new ArgumentNullException(nameof(editorProvider));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (!editorProvider.CanCreateNew)
                throw new NotSupportedException("The editor provider cannot create new documents.");

            return await PresentAndInitializeAsync(
                editorProvider,
                null,
                document => editorProvider.New(document, name));
        }

        private IDocument FindOpenDocument(string path)
        {
            if (_shell.Documents == null)
                return null;

            var fullPath = Path.GetFullPath(path);
            return _shell.Documents
                .OfType<IPersistedDocument>()
                .FirstOrDefault(document =>
                    !string.IsNullOrEmpty(document.FilePath) &&
                    string.Equals(
                        Path.GetFullPath(document.FilePath),
                        fullPath,
                        StringComparison.OrdinalIgnoreCase));
        }

        private async Task<IDocument> PresentAndInitializeAsync(
            IEditorProvider editorProvider,
            Action<IDocument> configure,
            Func<IDocument, Task> initialize)
        {
            var document = editorProvider.Create();
            if (document == null)
                throw new InvalidOperationException("The editor provider returned no document.");

            var viewAware = document as IViewAware;
            if (viewAware == null)
                throw new InvalidOperationException("Editor documents must implement IViewAware.");

            // Subscribe before presentation because the shell can attach or load the view
            // synchronously. The scope owns every temporary handler through both tasks.
            using (var readiness = new EditorViewReadiness(viewAware))
            {
                configure?.Invoke(document);

                var presentationTask = _shell.OpenDocumentAsync(document);
                if (presentationTask == null)
                    throw new InvalidOperationException("The shell returned no document presentation task.");

                await AwaitViewReadinessAsync(readiness.Task, presentationTask);

                var initializationTask = initialize(document);
                if (initializationTask == null)
                    throw new InvalidOperationException("The editor provider returned no initialization task.");

                await AwaitBothAsync(presentationTask, initializationTask);
            }

            return document;
        }

        private static async Task AwaitViewReadinessAsync(
            Task viewReadiness,
            Task presentation)
        {
            if (!viewReadiness.IsCompleted)
            {
                var completedTask = await Task.WhenAny(viewReadiness, presentation);
                if (ReferenceEquals(completedTask, presentation))
                    await presentation;
            }

            await viewReadiness;

            if (presentation.IsCompleted)
                await presentation;
        }

        private static async Task AwaitBothAsync(Task first, Task second)
        {
            await Task.WhenAll(first, second);
        }

        private sealed class EditorViewReadiness : IDisposable
        {
            private readonly IViewAware _viewAware;
            private readonly TaskCompletionSource<object> _completion;
            private FrameworkElement _view;
            private bool _isDisposed;

            public EditorViewReadiness(IViewAware viewAware)
            {
                _viewAware = viewAware;
                _completion = new TaskCompletionSource<object>(
                    TaskCreationOptions.RunContinuationsAsynchronously);

                _viewAware.ViewAttached += OnViewAttached;
                try
                {
                    ObserveView(_viewAware.GetView());
                }
                catch
                {
                    Dispose();
                    throw;
                }
            }

            public Task Task
            {
                get { return _completion.Task; }
            }

            public void Dispose()
            {
                if (_isDisposed)
                    return;

                _isDisposed = true;
                DetachHandlers();
            }

            private void OnViewAttached(object sender, ViewAttachedEventArgs eventArgs)
            {
                ObserveView(eventArgs.View);
            }

            private void ObserveView(object view)
            {
                if (_isDisposed || _completion.Task.IsCompleted || view == null)
                    return;

                var frameworkElement = view as FrameworkElement;
                if (frameworkElement == null)
                {
                    Fail(new InvalidOperationException(
                        "Editor views must derive from FrameworkElement."));
                    return;
                }

                if (!ReferenceEquals(_view, frameworkElement))
                {
                    DetachLoadedHandler();
                    _view = frameworkElement;
                }

                if (_view.IsLoaded)
                {
                    Complete();
                    return;
                }

                _view.Loaded -= OnViewLoaded;
                _view.Loaded += OnViewLoaded;

                if (_view.IsLoaded)
                    Complete();
            }

            private void OnViewLoaded(object sender, RoutedEventArgs eventArgs)
            {
                if (ReferenceEquals(sender, _view))
                    Complete();
            }

            private void Complete()
            {
                if (_completion.TrySetResult(null))
                    DetachHandlers();
            }

            private void Fail(Exception exception)
            {
                if (_completion.TrySetException(exception))
                    DetachHandlers();
            }

            private void DetachHandlers()
            {
                _viewAware.ViewAttached -= OnViewAttached;
                DetachLoadedHandler();
            }

            private void DetachLoadedHandler()
            {
                if (_view == null)
                    return;

                _view.Loaded -= OnViewLoaded;
                _view = null;
            }
        }
    }
}
