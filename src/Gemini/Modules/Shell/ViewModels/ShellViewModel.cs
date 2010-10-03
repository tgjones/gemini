using System;
using System.Collections.Generic;
using Caliburn.Core.Invocation;
using Caliburn.PresentationFramework.ApplicationModel;
using Gemini.Framework;
using Gemini.Framework.Questions;
using Gemini.Framework.Ribbon;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.Views;

namespace Gemini.Modules.Shell.ViewModels
{
	public class ShellViewModel : ScreenCollectionConductor, IShell
	{
		private string _title;
		private IShellView _shellView;
		private readonly IRibbon _ribbon;
		private readonly IStatusBar _statusBar;
		private readonly IDispatcher _dispatcher;

		public event EventHandler ActiveDocumentChanged;

		public string Title
		{
			get { return _title; }
			set { _title = value; NotifyOfPropertyChange("Title"); }
		}

		public IRibbon Ribbon
		{
			get { return _ribbon; }
		}

		public IStatusBar StatusBar
		{
			get { return _statusBar; }
		}

		public ShellViewModel(IRibbon ribbon, IStatusBar statusBar, IDispatcher dispatcher)
		{
			_ribbon = ribbon;
			_statusBar = statusBar;
			_dispatcher = dispatcher;
		}

		public void ShowTool(Pane pane, IExtendedPresenter model)
		{
			_dispatcher.ExecuteOnUIThread(() => _shellView.ShowTool(pane, model));
		}

		public void OpenDocument(IExtendedPresenter model)
		{
			Open(model, success =>
			{
				var index = Presenters.IndexOf(model);
				if (index == -1)
					_dispatcher.ExecuteOnUIThread(() => _shellView.OpenDocument(model));
				else _dispatcher.ExecuteOnUIThread(() => _shellView.OpenDocument((IExtendedPresenter) Presenters[index]));
			});
		}

		public void CloseDocument(IExtendedPresenter document, Action<bool> completed)
		{
			Shutdown(document, completed);
		}

		public void ActivateDocument(IExtendedPresenter document)
		{
			CurrentPresenter = document;
		}

		public event EventHandler AfterViewLoaded = delegate { };

		public void OnModulesInitialized(EventArgs args)
		{
			_dispatcher.ExecuteOnUIThread(() => _shellView.InitializeRibbon(Ribbon));
		}

		public override void ViewLoaded(object view, object context)
		{
			_shellView = (IShellView) view;
			AfterViewLoaded(this, EventArgs.Empty);
		}

		protected override void ExecuteShutdownModel(ISubordinate model, Action completed)
		{
			model.Execute(completed);
		}

		protected override void FinalizeShutdown(bool canShutdown, IEnumerable<IPresenter> allowedToShutdown)
		{
			if (canShutdown)
				base.FinalizeShutdown(true, allowedToShutdown);
		}
	}
}