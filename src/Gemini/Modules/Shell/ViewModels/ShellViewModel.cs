using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Framework.Themes;
using Gemini.Modules.MainMenu;
using Gemini.Modules.Shell.Services;
using Gemini.Modules.Shell.Views;
using Gemini.Modules.StatusBar;
using Gemini.Modules.ToolBars;

namespace Gemini.Modules.Shell.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : Conductor<IDocument>.Collection.OneActive, IShell
    {
        public event EventHandler ActiveDocumentChanging;
        public event EventHandler ActiveDocumentChanged;

#pragma warning disable 649
        [ImportMany(typeof(IModule))]
        private IEnumerable<IModule> _modules;

        [Import]
        private IThemeManager _themeManager;

        [Import]
        private IMenu _mainMenu;

        [Import]
        private IToolBars _toolBars;

        [Import]
        private IStatusBar _statusBar;

        [Import]
        private ILayoutItemStatePersister _layoutItemStatePersister;
#pragma warning restore 649

        private readonly TransitionCoordinator _transitionCoordinator;
        private readonly TaskCompletionSource<object> _initializationCompletion;
        private readonly BindableCollection<ITool> _tools;
        private IShellView _shellView;
        private Task _initializationRunner;
        private ILayoutItem _activeLayoutItem;
        // AvalonDock writes a transition's PropertyChanged value back synchronously.
        // Suppress only that echo; the same value arriving later can be a real request.
        private ILayoutItem _activeLayoutItemEcho;
        private bool _closing;

        public ShellViewModel()
        {
            _transitionCoordinator = new TransitionCoordinator();
            _initializationCompletion = new TaskCompletionSource<object>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            _tools = new BindableCollection<ITool>();
        }

        public IMenu MainMenu => _mainMenu;

        public IToolBars ToolBars => _toolBars;

        public IStatusBar StatusBar => _statusBar;

        public ILayoutItem ActiveLayoutItem
        {
            get => _activeLayoutItem;
            set
            {
                if (ReferenceEquals(_activeLayoutItemEcho, value))
                    return;

                ObserveBindingTransition(QueueTransition(
                    () => ApplyActiveLayoutItemBindingAsync(value)));
            }
        }

        public IObservableCollection<ITool> Tools => _tools;

        public IObservableCollection<IDocument> Documents => Items;

        public Task InitializationTask => _initializationCompletion.Task;

        private bool _showFloatingWindowsInTaskbar;
        public bool ShowFloatingWindowsInTaskbar
        {
            get => _showFloatingWindowsInTaskbar;
            set
            {
                _showFloatingWindowsInTaskbar = value;
                NotifyOfPropertyChange(() => ShowFloatingWindowsInTaskbar);
                if (_shellView != null)
                    _shellView.UpdateFloatingWindows();
            }
        }

        public virtual string StateFile => @".\ApplicationState.bin";

        public bool HasPersistedState => File.Exists(StateFile);

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            if (_initializationRunner != null)
                return;

            _initializationRunner = CompleteInitializationAsync((IShellView)view);
        }

        private async Task CompleteInitializationAsync(IShellView shellView)
        {
            try
            {
                await InitializeShellAsync(shellView);
                _initializationCompletion.TrySetResult(null);
            }
            catch (OperationCanceledException)
            {
                _initializationCompletion.TrySetCanceled();
            }
            catch (Exception exception)
            {
                _initializationCompletion.TrySetException(exception);
            }
        }

        private async Task InitializeShellAsync(IShellView shellView)
        {
            var modules = _modules.ToArray();

            foreach (var module in modules)
            {
                foreach (var globalResourceDictionary in module.GlobalResourceDictionaries)
                    Application.Current.Resources.MergedDictionaries.Add(globalResourceDictionary);
            }

            foreach (var module in modules)
                module.PreInitialize();
            foreach (var module in modules)
                module.Initialize();

            if (_themeManager.CurrentTheme == null)
            {
                if (!_themeManager.SetCurrentTheme(Properties.Settings.Default.ThemeName))
                {
                    Properties.Settings.Default.ThemeName =
                        (string)Properties.Settings.Default.Properties["ThemeName"].DefaultValue;
                    Properties.Settings.Default.Save();
                    if (!_themeManager.SetCurrentTheme(Properties.Settings.Default.ThemeName))
                        throw new InvalidOperationException("unable to load application theme");
                }
            }

            _shellView = shellView;
            var restoreResult = await _layoutItemStatePersister.LoadStateAsync(
                this,
                _shellView,
                StateFile);

            if (restoreResult.Status != LayoutItemStateLoadStatus.Success
                && restoreResult.Status != LayoutItemStateLoadStatus.NotFound)
            {
                OnStateRestoreWarning(restoreResult);
            }

            switch (restoreResult.Status)
            {
                case LayoutItemStateLoadStatus.Success:
                case LayoutItemStateLoadStatus.Partial:
                    await RestoreLayoutAsync(restoreResult.RestorePlan);
                    break;

                case LayoutItemStateLoadStatus.NotFound:
                case LayoutItemStateLoadStatus.Failed:
                case LayoutItemStateLoadStatus.Corrupt:
                    await OpenDefaultLayoutAsync(modules);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var module in modules)
                await module.PostInitializeAsync();
        }

        public bool RegisterTool(ITool tool)
        {
            if (Tools.Contains(tool))
                return false;

            Tools.Add(tool);
            return true;
        }

        public Task ShowToolAsync<TTool>()
            where TTool : ITool
        {
            return ShowToolAsync(IoC.Get<TTool>());
        }

        public Task ShowToolAsync(ITool model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return QueueTransition(() => ShowToolCoreAsync(model));
        }

        public Task CloseToolAsync(ITool tool)
        {
            if (tool == null)
                throw new ArgumentNullException(nameof(tool));

            return QueueTransition(() => CloseToolCoreAsync(tool));
        }

        public Task OpenDocumentAsync(IDocument model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return QueueTransition(
                () => ActivateItemAsync(model, CancellationToken.None));
        }

        public Task CloseDocumentAsync(IDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            return QueueTransition(
                () => DeactivateItemAsync(document, true, CancellationToken.None));
        }

        public override async Task ActivateItemAsync(
            IDocument item,
            CancellationToken cancellationToken)
        {
            if (_closing || ReferenceEquals(item, ActiveItem))
                return;

            RaiseActiveDocumentChanging();
            await base.ActivateItemAsync(item, cancellationToken);
            RaiseActiveDocumentChanged();
        }

        protected override void OnActivationProcessed(IDocument item, bool success)
        {
            if (success)
                SetActiveLayoutItemFromTransition(item);

            base.OnActivationProcessed(item, success);
        }

        public override async Task DeactivateItemAsync(
            IDocument item,
            bool close,
            CancellationToken cancellationToken)
        {
            RaiseActiveDocumentChanging();
            await base.DeactivateItemAsync(item, close, cancellationToken);
            RaiseActiveDocumentChanged();
        }

        protected override async Task OnDeactivateAsync(
            bool close,
            CancellationToken cancellationToken)
        {
            // Workaround for a complex bug that occurs when
            // (a) the window has multiple documents open, and
            // (b) the last document is NOT active
            // 
            // The issue manifests itself with a crash in
            // the call to base.ActivateItem(item), above,
            // saying that the collection can't be changed
            // in a CollectionChanged event handler.
            // 
            // The issue occurs because:
            // - Caliburn.Micro sees the window is closing, and calls Items.Clear()
            // - AvalonDock handles the CollectionChanged event, and calls Remove()
            //   on each of the open documents.
            // - If removing a document causes another to become active, then AvalonDock
            //   sets a new ActiveContent.
            // - We have a WPF binding from Caliburn.Micro's ActiveItem to AvalonDock's
            //   ActiveContent property, so ActiveItem gets updated.
            // - The document no longer exists in Items, beacuse that collection was cleared,
            //   but Caliburn.Micro helpfully adds it again - which causes the crash.
            //
            // My workaround is to use the following _closing variable, and ignore activation
            // requests that occur when _closing is true.
            _closing = true;

            var saveResult = _layoutItemStatePersister.SaveState(this, _shellView, StateFile);
            if (saveResult.Status != LayoutItemStateSaveStatus.Success)
                OnStateSaveWarning(saveResult);

            await base.OnDeactivateAsync(close, cancellationToken);
        }

        public void Close()
        {
            Application.Current.MainWindow.Close();
        }

        /// <summary>
        /// Handles faults from the fire-and-forget two-way binding setter path.
        /// </summary>
        protected virtual void OnBindingTransitionError(Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }

        protected virtual void OnStateRestoreWarning(LayoutItemStateLoadResult result)
        {
            Trace.TraceWarning(FormatPersistenceWarning("Layout state restore", result.Details, result.Exception));
        }

        /// <summary>
        /// Reports a partial or failed transactional state save.
        /// </summary>
        protected virtual void OnStateSaveWarning(LayoutItemStateSaveResult result)
        {
            Trace.TraceWarning(FormatPersistenceWarning("Layout state save", result.Details, result.Exception));
        }

        private Task QueueTransition(Func<Task> transition)
        {
            return _transitionCoordinator.Enqueue(transition);
        }

        private async Task OpenDefaultLayoutAsync(IEnumerable<IModule> modules)
        {
            foreach (var defaultDocument in modules.SelectMany(x => x.DefaultDocuments))
                await OpenDocumentAsync(defaultDocument);
            foreach (var defaultTool in modules.SelectMany(x => x.DefaultTools))
                await ShowToolAsync((ITool)IoC.GetInstance(defaultTool, null));
        }

        private async Task RestoreLayoutAsync(LayoutItemStateRestorePlan restorePlan)
        {
            foreach (var document in restorePlan.Documents)
                RegisterDocument(document);

            foreach (var tool in restorePlan.VisibleTools)
            {
                RegisterTool(tool);
                await QueueTransition(() => ActivateRestoredToolAsync(tool));
            }

            if (restorePlan.SelectedItem != null)
            {
                await QueueTransition(
                    () => ApplyRestoredSelectionAsync(restorePlan.SelectedItem));
            }
        }

        private bool RegisterDocument(IDocument document)
        {
            if (Documents.Any(existing => ReferenceEquals(existing, document)))
                return false;

            Items.Add(document);
            return true;
        }

        private async Task ApplyRestoredSelectionAsync(ILayoutItem item)
        {
            var document = item as IDocument;
            if (document != null)
            {
                await ActivateItemAsync(document, CancellationToken.None);
                document.IsSelected = true;
                return;
            }

            var tool = item as ITool;
            if (tool == null || !tool.IsVisible)
                return;

            if (!tool.IsActive)
                await tool.ActivateAsync(CancellationToken.None);
            tool.IsSelected = true;
            SetActiveLayoutItemFromTransition(tool);
        }

        private static string FormatPersistenceWarning(
            string operation,
            string details,
            Exception exception)
        {
            var message = string.IsNullOrEmpty(details) ? operation + " did not succeed." : details;
            return exception == null ? message : message + Environment.NewLine + exception;
        }

        private async Task ApplyActiveLayoutItemBindingAsync(ILayoutItem item)
        {
            SetActiveLayoutItemFromTransition(item);

            if (item is IDocument document)
                await ActivateItemAsync(document, CancellationToken.None);
        }

        private async Task ShowToolCoreAsync(ITool model)
        {
            RegisterTool(model);

            if (!model.IsActive)
                await model.ActivateAsync(CancellationToken.None);

            model.IsVisible = true;
            model.IsSelected = true;
            SetActiveLayoutItemFromTransition(model);
        }

        private async Task CloseToolCoreAsync(ITool tool)
        {
            if (!await tool.CanCloseAsync(CancellationToken.None))
                return;

            await tool.DeactivateAsync(true, CancellationToken.None);

            tool.IsVisible = false;
            tool.IsSelected = false;
            if (ReferenceEquals(_activeLayoutItem, tool))
                SetActiveLayoutItemFromTransition(null);
        }

        private static async Task ActivateRestoredToolAsync(ITool tool)
        {
            if (!tool.IsActive)
                await tool.ActivateAsync(CancellationToken.None);
        }

        private void SetActiveLayoutItemFromTransition(ILayoutItem item)
        {
            if (ReferenceEquals(_activeLayoutItem, item))
                return;

            _activeLayoutItemEcho = item;
            try
            {
                _activeLayoutItem = item;
                NotifyOfPropertyChange(() => ActiveLayoutItem);
            }
            finally
            {
                _activeLayoutItemEcho = null;
            }
        }

        private async void ObserveBindingTransition(Task transition)
        {
            try
            {
                await transition;
            }
            catch (Exception exception)
            {
                OnBindingTransitionError(exception);
            }
        }

        private void RaiseActiveDocumentChanging()
        {
            ActiveDocumentChanging?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseActiveDocumentChanged()
        {
            ActiveDocumentChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
