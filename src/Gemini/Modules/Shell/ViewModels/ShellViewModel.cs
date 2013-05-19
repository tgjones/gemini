using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Controls;
using Gemini.Framework.Menus;
using Gemini.Framework.Services;
using Gemini.Framework.ToolBars;
using Gemini.Modules.Shell.Views;

namespace Gemini.Modules.Shell.ViewModels
{
	[Export(typeof(IShell))]
	public class ShellViewModel : Conductor<IDocument>.Collection.OneActive, IShell
	{
		[ImportMany(typeof(IModule))]
		private IEnumerable<IModule> _modules;

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
		}

		protected override void OnViewLoaded(object view)
		{
            foreach (var module in _modules)
                module.PreInitialize();
			foreach (var module in _modules)
				module.Initialize();
            foreach (var module in _modules)
                module.PostInitialize();

            // TODO: Ideally, the ToolBarTray control would expose ToolBars
            // as a dependency property. We could use an attached property
            // to workaround this. But for now, toolbars need to be
            // created prior to the following code being run.
            foreach (var toolBar in ToolBars)
                ((IShellView) view).ToolBarTray.ToolBars.Add(new ToolBar
                {
                    ItemsSource = toolBar
                });

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

		public void Close()
		{
			Application.Current.MainWindow.Close();
		}
	}
}