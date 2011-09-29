using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.Views;

namespace Gemini.Modules.Shell.ViewModels
{
	[Export(typeof(IShell))]
	public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IShell
	{
		private IShellView _shellView;

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

		protected override void OnViewLoaded(object view)
		{
			_shellView = (IShellView) view;
			foreach (var module in _modules)
				module.Initialize();
			base.OnViewLoaded(view);
		}

		public void ShowTool(PaneLocation pane, IScreen model)
		{
			Execute.OnUIThread(() => _shellView.ShowTool(pane, model));
		}

		public void OpenDocument(IScreen model)
		{
			ActivateItem(model);
			Execute.OnUIThread(() => _shellView.OpenDocument(model));
		}

		public void CloseDocument(IScreen document)
		{
			DeactivateItem(document, true);
		}

		public void ActivateDocument(IScreen document)
		{
			ActivateItem(document);
		}

		public void Close()
		{
			Application.Current.MainWindow.Close();
		}
	}
}