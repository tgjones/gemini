using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.MainMenu;
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

		[ImportMany(typeof(IModule))]
		private IEnumerable<IModule> _modules;

	    private bool _closing;

		private WindowState _windowState = WindowState.Normal;
		public WindowState WindowState
		{
			get { return _windowState; }
			set
			{
				_windowState = value;
				NotifyOfPropertyChange(() => WindowState);
			}
		}

		private string _title = "[Default Title]";
		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				NotifyOfPropertyChange(() => Title);
			}
		}

		private ImageSource _icon;
		public ImageSource Icon
		{
			get { return _icon; }
			set
			{
				_icon = value;
				NotifyOfPropertyChange(() => Icon);
			}
		}

		[Import]
		private IMenu _mainMenu;
		public IMenu MainMenu
		{
			get { return _mainMenu; }
		}

        [Import]
        private IToolBars _toolBars;
        public IToolBars ToolBars
        {
            get { return _toolBars; }
        }

		[Import]
		private IStatusBar _statusBar;
		public IStatusBar StatusBar
		{
			get { return _statusBar; }
		}

		private readonly BindableCollection<ITool> _tools;
		public IObservableCollection<ITool> Tools
		{
			get { return _tools; }
		}

		public IObservableCollection<IDocument> Documents
		{
			get { return Items; }
		}

		public ShellViewModel()
		{
			_tools = new BindableCollection<ITool>();

		    if (!LayoutUtility.HasPersistedLayout)
		    {
		        // This workaround is necessary until https://avalondock.codeplex.com/workitem/15577
		        // is applied, or the bug is fixed in another way.
		        _tools.Add(new DummyTool(PaneLocation.Left));
		        _tools.Add(new DummyTool(PaneLocation.Right));
		        _tools.Add(new DummyTool(PaneLocation.Bottom));
		    }
		}

        [Export(typeof(DummyTool))]
        private class DummyTool : Tool
        {
            private readonly PaneLocation _preferredLocation;

            public override PaneLocation PreferredLocation
            {
                get { return _preferredLocation; }
            }

            public DummyTool(PaneLocation preferredLocation)
            {
                _preferredLocation = preferredLocation;
                IsVisible = false;
            }

            private DummyTool() {}
        }

		protected override void OnViewLoaded(object view)
		{
            foreach (var module in _modules)
                module.PreInitialize();
			foreach (var module in _modules)
				module.Initialize();

            var shellView = (IShellView) view;
		    if (!LayoutUtility.HasPersistedLayout)
		        foreach (var defaultTool in _modules.SelectMany(x => x.DefaultTools))
		            ShowTool((ITool) IoC.GetInstance(defaultTool, null));
		    else
		        shellView.LoadLayout();

            foreach (var module in _modules)
                module.PostInitialize();

		    base.OnViewLoaded(view);
		}

		public void ShowTool(ITool model)
		{
		    if (Tools.Contains(model))
		        model.IsVisible = true;
		    else
		        Tools.Add(model);
		    model.IsSelected = true;
		}

		public void OpenDocument(IDocument model)
		{
			ActivateItem(model);
		}

		public void CloseDocument(IDocument document)
		{
			DeactivateItem(document, true);
		}

		public void ActivateDocument(IDocument document)
		{
			ActivateItem(document);
		}

        public override void ActivateItem(IDocument item)
        {
            if (_closing)
                return;

            var handler = ActiveDocumentChanging;
            if (handler != null)
                handler(this, EventArgs.Empty);

            base.ActivateItem(item);
        }

        protected override void OnActivationProcessed(IDocument item, bool success)
        {
            var handler = ActiveDocumentChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);

            base.OnActivationProcessed(item, success);
        }

        protected override void OnDeactivate(bool close)
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

            base.OnDeactivate(close);
        }

		public void Close()
		{
			Application.Current.MainWindow.Close();
		}
	}
}