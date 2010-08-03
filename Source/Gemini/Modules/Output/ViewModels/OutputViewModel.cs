using System;
using AvalonDock;
using Caliburn.Core.Invocation;
using Gemini.Framework;
using Gemini.Modules.Output.Views;

namespace Gemini.Modules.Output.ViewModels
{
	public class OutputViewModel : Screen, IOutput
	{
		private readonly IDispatcher _dispatcher;
		private string _text = string.Empty;
		private IOutputView _view;

		public OutputViewModel(IDispatcher dispatcher)
		{
			_dispatcher = dispatcher;
		}

		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;

				NotifyOfPropertyChange("Text");

				if (_view != null)
					_dispatcher.ExecuteOnUIThread(() => _view.ScrollToEnd());
			}
		}

		public void Clear()
		{
			Text = string.Empty;
		}

		public void Append(string text)
		{
			Text += text + Environment.NewLine;
		}

		public override void ViewLoaded(object view, object context)
		{
			_view = (IOutputView) ((DockableContent) view).Content;
			_view.SetText(Text);
			_view.ScrollToEnd();
		}
	}
}