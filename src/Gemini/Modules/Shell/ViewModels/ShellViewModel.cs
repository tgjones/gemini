using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Gemini.Framework.Services;

namespace Gemini.Modules.Shell.ViewModels
{
	[Export(typeof(IShell))]
	public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IShell
	{
		[ImportMany(typeof(IModule))]
		private IEnumerable<IModule> _modules;

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

		public ShellViewModel()
		{
			_tools = new BindableCollection<ITool>();
		}

		protected override void OnViewLoaded(object view)
		{
			foreach (var module in _modules)
				module.Initialize();
			base.OnViewLoaded(view);
		}

		public void ShowTool(PaneLocation pane, ITool model)
		{
			Tools.Add(model);
			//Execute.OnUIThread(() => _shellView.ShowTool(pane, model));
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