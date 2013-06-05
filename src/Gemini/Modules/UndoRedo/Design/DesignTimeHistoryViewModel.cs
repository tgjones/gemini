using Gemini.Modules.UndoRedo.ViewModels;

namespace Gemini.Modules.UndoRedo.Design
{
    public class DesignTimeHistoryViewModel : HistoryViewModel
    {
        public DesignTimeHistoryViewModel()
            : base(null)
        {
            HistoryItems.Add(new HistoryItemViewModel("Initial", HistoryItemType.InitialState));
            HistoryItems.Add(new HistoryItemViewModel("Foo", HistoryItemType.Undo));
            HistoryItems.Add(new HistoryItemViewModel("Bar", HistoryItemType.Current));
            HistoryItems.Add(new HistoryItemViewModel("Baz", HistoryItemType.Redo));
        }
    }
}