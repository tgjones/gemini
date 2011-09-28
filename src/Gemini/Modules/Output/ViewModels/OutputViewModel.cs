using System;
using System.ComponentModel.Composition;
using AvalonDock;
using Caliburn.Micro;
using Gemini.Modules.Output.Views;

namespace Gemini.Modules.Output.ViewModels
{
	[Export(typeof(IOutput))]
	public class OutputViewModel : Screen, IOutput
	{
		private IOutputView _view;

		public override string DisplayName
		{
			get { return "Output"; }
		}

		private string _text = string.Empty;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;

				NotifyOfPropertyChange(() => Text);

				if (_view != null)
					Execute.OnUIThread(() => _view.ScrollToEnd());
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

		protected override void OnViewLoaded(object view)
		{
			_view = (IOutputView)((DockableContent)view).Content;
			_view.SetText(Text);
			_view.ScrollToEnd();
		}
	}
}