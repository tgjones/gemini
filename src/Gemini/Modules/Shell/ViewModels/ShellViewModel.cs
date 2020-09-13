using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

        private IShellView _shellView;
        private bool _closing;

        public IMenu MainMenu => _mainMenu;

        public IToolBars ToolBars => _toolBars;

        public IStatusBar StatusBar => _statusBar;

        private ILayoutItem _activeLayoutItem;
        public ILayoutItem ActiveLayoutItem
        {
            get => _activeLayoutItem;
            set
            {
                if (ReferenceEquals(_activeLayoutItem, value))
                    return;

                _activeLayoutItem = value;

                if (value is IDocument)
                    ActivateItemAsync((IDocument)value, CancellationToken.None).Wait();

                NotifyOfPropertyChange(() => ActiveLayoutItem);
            }
        }

        private readonly BindableCollection<ITool> _tools;
        public IObservableCollection<ITool> Tools => _tools;

        public IObservableCollection<IDocument> Documents => Items;

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

        public ShellViewModel()
        {
            ((IActivate)this).ActivateAsync(CancellationToken.None).Wait();

            _tools = new BindableCollection<ITool>();
        }

        protected override void OnViewLoaded(object view)
        {
            foreach (var module in _modules)
                foreach (var globalResourceDictionary in module.GlobalResourceDictionaries)
                    Application.Current.Resources.MergedDictionaries.Add(globalResourceDictionary);

            foreach (var module in _modules)
                module.PreInitialize();
            foreach (var module in _modules)
                module.Initialize();

            // If after initialization no theme was loaded, load the default one
            if (_themeManager.CurrentTheme == null)
            {
                if (!_themeManager.SetCurrentTheme(Properties.Settings.Default.ThemeName))
                {
                    Properties.Settings.Default.ThemeName = (string)Properties.Settings.Default.Properties["ThemeName"].DefaultValue;
                    Properties.Settings.Default.Save();
                    if (!_themeManager.SetCurrentTheme(Properties.Settings.Default.ThemeName))
                    {
                        throw new InvalidOperationException("unable to load application theme");
                    }
                }
            }

            _shellView = (IShellView)view;

            Execute.OnUIThreadAsync(async () =>
            {
                if (!_layoutItemStatePersister.LoadState(this, _shellView, StateFile))
                {
                    foreach (var defaultDocument in _modules.SelectMany(x => x.DefaultDocuments))
                        await OpenDocumentAsync(defaultDocument);
                    foreach (var defaultTool in _modules.SelectMany(x => x.DefaultTools))
                        ShowTool((ITool)IoC.GetInstance(defaultTool, null));
                }

                foreach (var module in _modules)
                    await module.PostInitializeAsync();
            });

            base.OnViewLoaded(view);
        }

        public void ShowTool<TTool>()
            where TTool : ITool
        {
            ShowTool(IoC.Get<TTool>());
        }

        public void ShowTool(ITool model)
        {
            if (Tools.Contains(model))
            {
                model.IsVisible = true;
            }
            else
            {
                Tools.Add(model);

                model.ActivateAsync(CancellationToken.None).Wait();
            }

            model.IsSelected = true;
            ActiveLayoutItem = model;
        }

        public Task OpenDocumentAsync(IDocument model) => ActivateItemAsync(model, CancellationToken.None);

        public Task CloseDocumentAsync(IDocument document) => DeactivateItemAsync(document, true, CancellationToken.None);

        private bool _activateItemGuard = false;

        public override async Task ActivateItemAsync(IDocument item, CancellationToken cancellationToken)
        {
            if (_closing || _activateItemGuard)
                return;

            _activateItemGuard = true;

            try
            {
                if (ReferenceEquals(item, ActiveItem))
                    return;

                RaiseActiveDocumentChanging();

                var currentActiveItem = ActiveItem;

                await base.ActivateItemAsync(item, cancellationToken);

                RaiseActiveDocumentChanged();
            }
            finally
            {
                _activateItemGuard = false;
            }
        }

        private void RaiseActiveDocumentChanging()
        {
            var handler = ActiveDocumentChanging;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void RaiseActiveDocumentChanged()
        {
            var handler = ActiveDocumentChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected override void OnActivationProcessed(IDocument item, bool success)
        {
            if (!ReferenceEquals(ActiveLayoutItem, item))
                ActiveLayoutItem = item;

            base.OnActivationProcessed(item, success);
        }

        public override async Task DeactivateItemAsync(IDocument item, bool close, CancellationToken cancellationToken)
        {
            RaiseActiveDocumentChanging();

            await base.DeactivateItemAsync(item, close, cancellationToken);

            RaiseActiveDocumentChanged();
        }

        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
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

            _layoutItemStatePersister.SaveState(this, _shellView, StateFile);

            await base.OnDeactivateAsync(close, cancellationToken);
        }

        public void Close()
        {
            Application.Current.MainWindow.Close();
        }
    }
}
