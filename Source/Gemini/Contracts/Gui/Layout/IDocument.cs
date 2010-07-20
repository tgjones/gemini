using System;
using System.ComponentModel;

namespace Gemini.Contracts.Gui.Layout
{
	public interface IDocument : ILayoutItem
	{
		void OnOpened(object sender, EventArgs e);
		void OnClosing(object sender, CancelEventArgs e);
		void OnClosed(object sender, EventArgs e);
		string Memento { get; }
		IDocument CreateDocument(string memento);
	}
}