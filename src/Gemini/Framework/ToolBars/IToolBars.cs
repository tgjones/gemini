using Caliburn.Micro;

namespace Gemini.Framework.ToolBars
{
    public interface IToolBars : IObservableCollection<IToolBar>
    {
        bool Visible { get; set; }
    }
}