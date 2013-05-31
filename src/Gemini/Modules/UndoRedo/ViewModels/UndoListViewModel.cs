using Caliburn.Micro;

namespace Gemini.Modules.UndoRedo.ViewModels
{
    public class UndoListViewModel : PropertyChangedBase
    {
        private readonly IObservableCollection<IUndoableAction> _items;

        public IObservableCollection<IUndoableAction> Items
        {
            get { return _items; }
        }

        private int _hoveredIndex;
        public int HoveredIndex
        {
            get { return _hoveredIndex; }
            set
            {
                _hoveredIndex = value;
                NotifyOfPropertyChange(() => HoveredIndex);
            }
        }

        public UndoListViewModel(IObservableCollection<IUndoableAction> items)
        {
            _items = items;
        }
    }
}