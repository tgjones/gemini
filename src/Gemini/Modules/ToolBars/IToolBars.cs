using Caliburn.Micro;

namespace Gemini.Modules.ToolBars
{
    public interface IToolBars
    {
        IObservableCollection<IToolBar> Items {get;}
        bool Visible { get; set; }
    }
}