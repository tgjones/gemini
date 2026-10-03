using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Modules.MainMenu;
using Gemini.Modules.StatusBar;
using Gemini.Modules.ToolBars;

namespace Gemini.Framework.Services
{
    public interface IShell : IGuardClose, IDeactivate
	{
        event EventHandler ActiveDocumentChanging;
        event EventHandler ActiveDocumentChanged;

        bool ShowFloatingWindowsInTaskbar { get; set; }
        
		IMenu MainMenu { get; }
        IToolBars ToolBars { get; }
		IStatusBar StatusBar { get; }

        // TODO: Rename this to ActiveItem.
        ILayoutItem ActiveLayoutItem { get; set; }

        // TODO: Rename this to SelectedDocument.
		IDocument ActiveItem { get; }

		IObservableCollection<IDocument> Documents { get; }
		IObservableCollection<ITool> Tools { get; }

        /// <summary>
        /// Completes after view-dependent shell setup, layout or default presentation,
        /// and module post-initialization; faults when startup fails.
        /// </summary>
        Task InitializationTask { get; }

        bool RegisterTool(ITool tool);
        /// <summary>Queues the complete lifecycle operation that shows a tool.</summary>
        Task ShowToolAsync<TTool>() where TTool : ITool;
        /// <summary>Queues the complete lifecycle operation that shows a tool.</summary>
		Task ShowToolAsync(ITool model);
        /// <summary>
        /// Queues guard evaluation and close lifecycle. A veto preserves the tool;
        /// success hides it without unregistering it.
        /// </summary>
        Task CloseToolAsync(ITool tool);

        /// <summary>Queues the complete lifecycle operation that opens a document.</summary>
		Task OpenDocumentAsync(IDocument model);
        /// <summary>Queues the complete lifecycle operation that closes a document.</summary>
		Task CloseDocumentAsync(IDocument document);

		void Close();
	}
}
