using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AvalonDock;

namespace Gemini.Contracts.Gui.Layout
{
	public interface ILayoutManager
	{
		event EventHandler Loaded;
		event EventHandler Unloading;
		event EventHandler LayoutUpdated;

		ReadOnlyCollection<IPad> Pads { get; }
		ReadOnlyCollection<IDocument> Documents { get; }

		void ShowPad(IPad pad, AnchorStyle anchor = AnchorStyle.Right);
		void HidePad(IPad pad);
		void HideAllPads();
		IDocument ShowDocument(IDocument document, string memento);
		void CloseDocument(IDocument document);
		void CloseAllDocuments();

		bool IsVisible(IPad pad);
		bool IsVisible(IDocument document);

		/// <summary>
		/// Called on recomposition by the Workbench
		/// </summary>
		void SetAllPadsDocuments(
				IEnumerable<Lazy<IPad, IPadMeta>> AllPads,
				IEnumerable<Lazy<IDocument, IDocumentMeta>> AllDocuments);

		/// <summary>
		/// Called by the workbench to notify the LayoutManager
		/// that we're about to unload
		/// </summary>
		void UnloadingWorkbench();

		string SaveLayout(); // returns a blob
		void RestoreLayout(string blob);
	}
}